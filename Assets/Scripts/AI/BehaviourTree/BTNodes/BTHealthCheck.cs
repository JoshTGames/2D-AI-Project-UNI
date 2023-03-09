using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.ItemStorage;
using JoshGames.CharacterController;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTHealthCheck : BTNode{
    AIBrain aI;       
    float percentage;

    CheckType checkType;

    float health, maxHealth;
    

    public BTHealthCheck(AIBrain _ai, float _percentage, CheckType _checkType = CheckType.LESS_THAN){ 
        this.aI = _ai;
        this.percentage = _percentage / 100;
        this.checkType = _checkType;
    }
    
    public override NodeState Evaluate(){             
        aI.GetHealth(out health, out maxHealth);
        float healthPercent = (health / maxHealth);
        return ((checkType == CheckType.LESS_THAN && (healthPercent <= percentage)) || (checkType == CheckType.GREATER_THAN && (healthPercent >= percentage)))? NodeState.Success : NodeState.Fail;
    }

    public enum CheckType{
        GREATER_THAN,
        LESS_THAN
    }
}
