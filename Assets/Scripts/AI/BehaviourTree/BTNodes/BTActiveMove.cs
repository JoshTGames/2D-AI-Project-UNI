using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;


/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
        THIS SCRIPT IS DIFFERENT TO THE PREVIOUS MOVE TASK. IT OPERATES BY TAKING THE TRANSFORM INSTEAD OF THE POSITION
*/

public class BTActiveMove : BTNode{
    BTNode task;
    AIBrain ai;
    AIMemory memory;
    Transform target;    
    Vector3 targetPosition;
    Vector3 previousPosition; // Previous position of the agent...
    float colliderRadius;

    bool isSeeking, faceTarget;
    float positionDelta;
    bool atPosition = false;
    bool doInvestigate = false;

    Vector3 boundsSize;
    public BTActiveMove(AIBrain _ai = null, AIMemory _memory = null, Transform _target = null, float _positionDelta = .1f, bool _isSeeking = true, bool _faceTarget = false, bool _doInvestigate = false){ 
        if(!_ai || !_target || !_memory){ return; } // STOPS CONTRUCTION IF NULL

        this.ai = _ai;        
        this.memory = _memory;
        this.target = _target;
        this.isSeeking = _isSeeking;
        this.faceTarget = _faceTarget;
        this.positionDelta = _positionDelta;
        this.doInvestigate = _doInvestigate;

        boundsSize = Vector3.Scale(((Vector3)ai.bounds.size / 2), ai.bounds.cellSize) * .999f;

        this.colliderRadius = Mathf.Max(ai.thisCollider.bounds.size.x, ai.thisCollider.bounds.size.y);        
    }
    public override NodeState Evaluate(){
        if(!ai){ return NodeState.Fail; } // SAFETY MEASURE        

        // Move to last seen position...
        bool isVisible = TargetIsVisible(target);

        targetPosition = ClampedPosition.GetClampedPos((isVisible)? target.position : targetPosition, ai.bounds.transform.position, boundsSize);    
        float distanceFromTarget = Vector3.Distance(targetPosition, ai.transform.position);
        atPosition = (isSeeking)? (distanceFromTarget <= positionDelta) : (distanceFromTarget >= positionDelta); // If agent is at target or is far enoguh from target...        


        ai.isSeeking = isSeeking; // Used to choose if agent is fleeing or not...
        
        ai.moveDirection = ai.steeringSolver.GetDirToMove(ai.steeringBehaviours, ai, (!atPosition)? targetPosition : ai.transform.position); // The direction to be applied onto the agent to move towards...
        ai.positionOfInterest = targetPosition;
        
        if(faceTarget){ return NodeState.Fail; }
        previousPosition = (ai.moveDirection != Vector3.zero)? ai.moveDirection : previousPosition;

        // Investigation
        if(atPosition && !isVisible && doInvestigate && task == null && target){ 
            int rndInvestigateVal = Random.Range(ai.agentProfile.directionsToInvestigate.min, 3 + 1);        
            BTNode task = new BTInvestigate(ai, 2, 5, .25f, .75f, rndInvestigateVal, targetPosition, true, target);
            
            if(AddInvestigateToMemory(target, task) != null){
                return NodeState.Success;
            }
        }
        
        return (atPosition)? NodeState.Success : NodeState.Running;
    }

    bool TargetIsVisible(Transform _target){
        foreach(Transform entity in ai.entitiesInSight){            
            if(_target.GetInstanceID() == entity.GetInstanceID()){ return true; }
        }         
        return false;
    }

    Vector3? GetTargetPosition(Vector3 _targetPosition, float _colliderRadius, LayerMask _whatIsObstacles){
        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)_targetPosition, _colliderRadius, _whatIsObstacles);      
        Collider2D hit = null;
        if(hits.Length >0){
            foreach(Collider2D _hit in hits){
                if(_hit.transform.GetInstanceID() == ai.transform.GetInstanceID()){ continue; }
                hit = _hit;
                break;
            }
        }
        if(hit){
            Vector2 directionFromObj = (Vector2)(_targetPosition - hit.bounds.center);                
            _targetPosition = hit.ClosestPoint((Vector2)_targetPosition) +  (directionFromObj.normalized * (_colliderRadius));
            Debug.DrawLine(hit.bounds.center, (Vector3)_targetPosition, Color.blue, 10f);                
            return _targetPosition;
        }    
        return null;
    }


    /// <summary>
    /// /// This function will add a task to memory for processing by the agent in the future
    /// </summary>
    /// <param name="transform">This is the object the task will be assigned around</param>
    /// <param name="task">This is the task which will drive the behaviour</param>
    int? AddInvestigateToMemory(Transform transform, BTNode task){
        if(task == null){ return null; } // If task holds no value...        
        
        // GET TASK SETTINGS
        float? taskLifetime = null;
        float taskPriority = 0;
        foreach(AISettings.Priorities tPriority in ai.agentProfile.priorities){
            if(tPriority.task == AISettings.TaskPriorities.Position){ 
                taskLifetime = tPriority.timeInMemory;
                taskPriority = tPriority.priority /2;
            }
        }

        // CHECK FOR EXISTING MEMORY DATA
        bool dataExists = false;
        /// <summary>
        /// This for each function will set 'dataExists' to true IF it has found a memory value which matches a value inside the sight of the ai...
        /// </summary>                    
        memory.activeMemory.ForEach(data => {
            if(data.recordedTarget == transform && data.taskPriority == AISettings.TaskPriorities.Position){ 
                data.lifetime = taskLifetime; // Updates the lifetime of the task
                data.recordedPosition = transform.position;
                data.task = task;
                dataExists = true;                             
            }
        });  
        if(dataExists){ return null; }
        // Create new memory data...
        memory.activeMemory.Add(
            new AIMemory.Memory(
                taskLifetime,
                taskPriority,
                AISettings.TaskPriorities.Position,
                transform.position,
                transform,
                task
            )
        );
        return memory.activeMemory.Count;
    }
}

