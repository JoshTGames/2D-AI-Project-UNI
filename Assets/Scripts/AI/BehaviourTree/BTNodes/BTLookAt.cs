using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTLookAt : BTNode{
    AIBrain ai;
    Vector3 dirToTarget;
    
    public BTLookAt(AIBrain _ai = null, Vector3? _dirToTarget = null){ 
        if(!_ai || _dirToTarget == null){ return; } // STOPS CONTRUCTION IF NULL

        this.ai = _ai;
        this.dirToTarget = (Vector3)_dirToTarget;        
    }
    
    public override NodeState Evaluate(){        
        ai.moveDirection = Vector3.zero;
        ai.positionOfInterest = dirToTarget;
        return NodeState.Success;
    }
}
