using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTWander : BTNode{   
    BTMove MOVE = new BTMove();
    AIBrain ai;
    float waitTimeRemaining;
    float maxWanderLength, timeDeductor, minWaitTime, maxWaitTime;
    Vector2? pivotPosition = null;
    Vector2? randomPosition; // Generated at start    

    /// <summary>
    /// This function will generate a random position based on a position to move towards... (DISCLAIMER: NEED TO CHECK THE RANDOM POSITION TO MAKE SURE ITS ACCESSIBLE)
    /// </summary>
    /// <returns>A generated random position</returns>
    Vector2 GeneratePosition() => ((pivotPosition == null)? ai.transform.position : (Vector3)pivotPosition) + (Vector3)new Vector2( // This will generate a random position either from the agent or from a pivot position based on whats parsed into the function
        Random.Range(-maxWanderLength, maxWanderLength),
        Random.Range(-maxWanderLength, maxWanderLength)
    );

    public BTWander(AIBrain _ai, float _maxWanderLength, float _timeDeductor, float _minWaitTime, float _maxWaitTime, Vector2? _pivotPosition = null){ 
        this.ai = _ai;
        this.maxWanderLength = _maxWanderLength;
        this.timeDeductor = _timeDeductor;
        this.minWaitTime = _minWaitTime;
        this.maxWaitTime = _maxWaitTime;
        this.pivotPosition = _pivotPosition;        

        this.waitTimeRemaining = Random.Range(_minWaitTime, _maxWaitTime);
    }

    public override NodeState Evaluate(){        
        if(randomPosition == null && waitTimeRemaining <= 0){ // If no position is available and has finished waiting...
            randomPosition = GeneratePosition();             
            waitTimeRemaining = Random.Range(minWaitTime, maxWaitTime);

            MOVE = new BTMove(ai, randomPosition); // Create a new position to move towards
        }          
        waitTimeRemaining -= (randomPosition == null)? timeDeductor : 0; // Negate time if there is no position to move to...          
       
        if(MOVE.Evaluate() == NodeState.Success){ // Simultaneously Check if the agent has successfully moved towards the target position and also calls upon the move node...
            randomPosition = null;
            return NodeState.Success;
        }
        
        return NodeState.Running;
    }
}
