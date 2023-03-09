using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.AI.Steering;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTInvestigate : BTNode{
    BTNode task;
    AIBrain ai;
    Vector3[] targetPositionsToInvestigate;
    float minDistanceToTravel, maxDistanceToTravel;
    float minWaitTime, maxWaitTime;
    bool limitedDirections;
    Vector3 targetPosition;
    Transform target;
    bool hasTarget;
    float chance;

    bool hasGenerated;
    public BTInvestigate(AIBrain _ai = null, float _minDistanceToTravel = 1f, float _maxDistanceToTravel = 1f, float _minWaitTime = 1, float _maxWaitTime = 1, int _positionsToInvestigate = 1, Vector3? _targetPosition = null, bool _limitedDirections = false, Transform _target = null){ 
        if(!_ai){ return; } // STOPS CONTRUCTION IF NULL

        this.ai = _ai;   
        this.targetPositionsToInvestigate = new Vector3[_positionsToInvestigate];
        this.minDistanceToTravel = _minDistanceToTravel;
        this.maxDistanceToTravel = _maxDistanceToTravel;
        this.minWaitTime = _minWaitTime;
        this.maxWaitTime = _maxWaitTime;
        this.targetPosition = (_targetPosition != null)? (Vector3)_targetPosition : ai.transform.position;  
        this.limitedDirections = _limitedDirections;
        this.target = _target;
        hasTarget = (target != null);
    }    


    void GeneratePositionsOfInterest(){
        List<Vector2> directionsToChooseFrom = SteeringBase.directionsAroundCharacter.ToList(); // Creates a manipulatable list of all the directions around character
        Dictionary<int, float> directionDistances = new Dictionary<int, float>();
        if(limitedDirections){ // get appropriate directions based on position of target in relation to the agent
            // Iterate through each position and calculate the distance from each direction to the target            
            for(int i = 0; i < directionsToChooseFrom.Count; i++){
                float distance = Vector3.Distance(targetPosition, ai.transform.position + (Vector3) directionsToChooseFrom[i]);                
                directionDistances.Add(i, distance);
            }        
            List<KeyValuePair<int, float>> newDirections = directionDistances.OrderBy((dir) => dir.Value).ToList();
            directionsToChooseFrom.Clear();
            for(int i = 0; i < targetPositionsToInvestigate.Length; i++){
                directionsToChooseFrom.Add(SteeringBase.directionsAroundCharacter[newDirections[i].Key]);
            }
        }

        for(int i = 0; i < targetPositionsToInvestigate.Length; i++){            
            int randomDir = Random.Range(0, directionsToChooseFrom.Count); // Get a random direction out of the list of directions
            float distance = Random.Range(minDistanceToTravel, maxDistanceToTravel);
            RaycastHit2D hit = Physics2D.Raycast(ai.thisCollider.ClosestPoint(ai.thisCollider.bounds.center + ((Vector3) directionsToChooseFrom[randomDir] * distance)), (Vector3) directionsToChooseFrom[randomDir], distance, ai.whatIsObstacles);
            
            Debug.DrawLine(ai.transform.position, ai.transform.position + ((Vector3)directionsToChooseFrom[randomDir] * distance), Color.white, 1f);
            targetPositionsToInvestigate[i] = (hit)? hit.point : ai.transform.position + ((Vector3) directionsToChooseFrom[randomDir] * distance);           
            
            directionsToChooseFrom.RemoveAt(randomDir); // Remove chosen direction from list
            this.chance = (float)Random.Range(0, 100)/100;
        }
    }
    
    public override NodeState Evaluate(){        
        if(!ai || (hasTarget && !target)){ return NodeState.Fail; } // SAFETY MEASURE       
        if(chance > ai.agentProfile.chanceToInvestigate){ return NodeState.Success; } // Means that there is a chance to investigate rather than immediate action    

        if(!hasGenerated){      
            hasGenerated = true;            
            GeneratePositionsOfInterest();            
            List<BTNode> tasks = new List<BTNode>();
           
            foreach(Vector3 targetPos in targetPositionsToInvestigate){ // Iterate through each selected position to move towards...                
                List<BTNode> FACE_DIRECTION = new List<BTNode>();
                List<Vector2> availableDirections = SteeringBase.directionsAroundCharacter.ToList(); // Creates a manipulatable list of all the directions around character
                int directionsToCheck = Random.Range(3, 5); // Get a random selection of those directions
                
                for(int i = 0; i < directionsToCheck; i++){
                    int rndDir = Random.Range(0, (limitedDirections && i == 0)? 0 : availableDirections.Count); // Get a random direction out of the list of directions

                    FACE_DIRECTION.Add(new BTSequence(new List<BTNode>{
                        new BTLookAt(ai, ai.transform.position + (Vector3) availableDirections[rndDir]),
                        new BTWait(ai, Random.Range(minWaitTime, maxWaitTime)                        
                    )}));
                    availableDirections.RemoveAt(rndDir); // Remove chosen direction from list
                }
                tasks.Add(new BTSequence(new List<BTNode>{
                    new BTMove(ai, targetPos),                    
                    new BTSequence(FACE_DIRECTION)
                }));
            }
            
            task = new BTSequence(tasks);
        }
        
        NodeState isSuccess = task.Evaluate();        
        return isSuccess;
    }
}
