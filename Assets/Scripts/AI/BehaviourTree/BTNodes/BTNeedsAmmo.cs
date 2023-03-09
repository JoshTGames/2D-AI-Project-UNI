using UnityEngine;
using System.Collections.Generic;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.ItemStorage;
using System.Linq;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTNeedsAmmo : BTNode{
    AIBrain ai;
    Inventory inv;
    Inventory_ItemStats ammo;

    public BTNeedsAmmo(AIBrain _ai, Inventory _inv, Inventory_ItemStats _ammo){ 
        this.ai = _ai;
        this.inv = _inv;
        this.ammo = _ammo;
    }   
    
    public override NodeState Evaluate(){        
        bool hasDemandingWeapon = false;
        inv.GetData(inv.tag).ToList().ForEach((data) => {
            Item item = data.Value[0];
            if(item.GetType() == typeof(Weapon) && item.weaponData.projectile?.name == ammo.GetItem().name){ hasDemandingWeapon = true; }
        });
        if(hasDemandingWeapon){ ai.itemToPickUpCache = ammo; }
        return (hasDemandingWeapon)? NodeState.Success : NodeState.Fail;
    }
}
