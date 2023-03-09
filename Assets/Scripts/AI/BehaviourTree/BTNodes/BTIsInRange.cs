using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.ItemStorage;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTIsInRange : BTNode{
    AIBrain aI;       
    Transform otherTransform;
    float range;

    public BTIsInRange(AIBrain _ai, Transform _otherTransform, float _range){ 
        this.aI = _ai;
        this.otherTransform = _otherTransform;
        this.range = _range;
    }
    
    public override NodeState Evaluate(){
        float distance = Vector3.Distance(aI.thisCollider.bounds.center, otherTransform.position);        
        return (distance <= range)? NodeState.Success : NodeState.Fail;
    }
}
