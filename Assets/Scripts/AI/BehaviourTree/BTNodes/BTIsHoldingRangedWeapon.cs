using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.ItemStorage;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTIsHoldingRangedWeapon : BTNode{
    AIBrain aI;

    public BTIsHoldingRangedWeapon(AIBrain _aI){ 
        this.aI = _aI;
    }
    
    public override NodeState Evaluate(){     
        return (aI.EquippedItem.weaponData.projectile)? NodeState.Success : NodeState.Fail;
    }
}
