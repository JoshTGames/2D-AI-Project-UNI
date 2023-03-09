using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTIsAtPosition : BTNode{
    AIBrain ai;
    Vector3 targetPosition;        
    public BTIsAtPosition(AIBrain _ai = null, Vector3? _targetPosition = null){ 
        if(!_ai || _targetPosition == null){ return; } // STOPS CONTRUCTION IF NULL

        this.ai = _ai;
        this.targetPosition = (Vector3)_targetPosition;        
    }
    
    public override NodeState Evaluate(){
        if(!ai){ return NodeState.Fail; } // SAFETY MEASURE        

        float distanceFromTarget = Vector3.Distance(targetPosition, ai.transform.position);
        
        bool atPosition = (ai.isSeeking)? (distanceFromTarget <= (ai.agentToTargetDelta * 2f)) : (distanceFromTarget >= ai.agentFromTargetDelta);

        return (atPosition) ? NodeState.Success : NodeState.Fail;
    }
}
