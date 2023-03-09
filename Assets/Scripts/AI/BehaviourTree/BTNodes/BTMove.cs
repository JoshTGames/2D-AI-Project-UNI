using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using UnityEngine.Tilemaps;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTMove : BTNode{
    AIBrain ai;
    Vector3 targetPosition;
    Vector3 previousPosition; // Previous position of the agent...
    bool isSeeking, faceTarget;
    float positionDelta;
    bool atPosition = false;
    public BTMove(AIBrain _ai = null, Vector3? _targetPosition = null, float _positionDelta = .1f, bool _isSeeking = true, bool _faceTarget = false){ 
        if(!_ai || _targetPosition == null){ return; } // STOPS CONTRUCTION IF NULL

        this.ai = _ai;        
        this.isSeeking = _isSeeking;
        this.positionDelta = _positionDelta;
        this.faceTarget = _faceTarget;

        if(isSeeking){
            float colliderRadius = Mathf.Max(ai.thisCollider.bounds.size.x, ai.thisCollider.bounds.size.y);
            _targetPosition = GetTargetPosition((Vector3)_targetPosition, colliderRadius, ai.whatIsObstacles);
        }

        Vector3 boundsSize = Vector3.Scale(((Vector3)ai.bounds.size / 2), ai.bounds.cellSize);
        this.targetPosition = ClampedPosition.GetClampedPos((Vector3)_targetPosition, ai.bounds.transform.position, boundsSize);        
    }
    
    public override NodeState Evaluate(){
        if(!ai){ return NodeState.Fail; } // SAFETY MEASURE    

        ai.isSeeking = isSeeking; // Used to choose if agent is fleeing or not...
        ai.moveDirection = ai.steeringSolver.GetDirToMove(ai.steeringBehaviours, ai, (!atPosition)? targetPosition : ai.transform.position); // The direction to be applied onto the agent to move towards...
        ai.positionOfInterest = (!atPosition)? ai.transform.position + previousPosition : ai.positionOfInterest; // The direction the agent should be looking at...

        if(atPosition && !faceTarget){ return NodeState.Success; }

        float distanceFromTarget = Vector3.Distance(targetPosition, ai.transform.position);
        atPosition = (isSeeking)? (distanceFromTarget <= positionDelta) : (distanceFromTarget >= positionDelta); // If agent is at target or is far enoguh from target...        
    
        previousPosition = (ai.moveDirection != Vector3.zero)? ai.moveDirection : previousPosition;        
        return (atPosition)? NodeState.Success : NodeState.Fail;
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
        }    
        return _targetPosition;
    }
}
