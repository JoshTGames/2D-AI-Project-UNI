using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTPatrol : BTNode{   
    BTMove MOVE = new BTMove();
    AIBrain ai;
    float waitTimeRemaining;
    float timeDeductor, minWaitTime, maxWaitTime;
    
    
    Vector2? selectedPosition;
    public BTPatrol(AIBrain _ai,float _timeDeductor, float _minWaitTime, float _maxWaitTime){ 
        this.ai = _ai;        
        this.timeDeductor = _timeDeductor;
        this.minWaitTime = _minWaitTime;
        this.maxWaitTime = _maxWaitTime;               

        this.waitTimeRemaining = Random.Range(_minWaitTime, _maxWaitTime);                        
    }

    public override NodeState Evaluate(){        
        if(ai.GetPatrolLength() <= 0){ return NodeState.Fail; } // If there are no patrols... Then there is no need to run this code

        if(selectedPosition == null && waitTimeRemaining <= 0){ // If no position is available and has finished waiting...
            selectedPosition = ai.GetPatrolPosition(ai.GetNextPatrolPosition()); 
            waitTimeRemaining = Random.Range(minWaitTime, maxWaitTime);

            MOVE = new BTMove(ai, selectedPosition); // Create a new position to move towards
        }          
        waitTimeRemaining -= (selectedPosition == null)? timeDeductor : 0; // Negate time if there is no position to move to...          

        if(MOVE.Evaluate() == NodeState.Success){ // Simultaneously Check if the agent has successfully moved towards the target position and also calls upon the move node...
            selectedPosition = null;
            return NodeState.Success;
        }
        
        return NodeState.Running;
    }
}
