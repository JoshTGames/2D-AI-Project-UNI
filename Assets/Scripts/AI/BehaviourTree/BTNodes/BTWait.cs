using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTWait : BTNode{
    AIBrain ai;
    float duration, timeToWait;
    bool hasActivated;
    
    public BTWait(AIBrain _ai, float _duration){ 
        this.ai = _ai;
        this.duration = _duration;  
    }
    
    public override NodeState Evaluate(){        
        if(!hasActivated){
            hasActivated = true;
            timeToWait = Time.time + duration;            
            ai.moveDirection = Vector3.zero;
        }
        return (Time.time >= timeToWait && hasActivated) ? NodeState.Success: NodeState.Fail;
    }
}
