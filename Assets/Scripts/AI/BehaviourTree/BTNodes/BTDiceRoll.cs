using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTDiceRoll : BTNode{
    AIBrain ai;
    float value;
    float chance;
    
    public BTDiceRoll(AIBrain _ai = null, float _value = 50){ 
        this.ai = _ai; 
        this.value = _value / 100;
        this.chance = (float)Random.Range(0, 100)/100;
    }   
    
    public override NodeState Evaluate(){        
        return (chance < value)? NodeState.Success : NodeState.Fail;
    }
}
