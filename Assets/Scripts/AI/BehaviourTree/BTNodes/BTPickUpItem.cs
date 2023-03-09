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

public class BTPickUpItem : BTNode{
    AIBrain ai;
    Inventory inv;
    Inventory_ItemStats item;

    public BTPickUpItem(AIBrain _ai, Inventory _inv, Inventory_ItemStats _item){ 
        this.ai = _ai;
        this.inv = _inv;
        this.item = _item;
    }
    
    public override NodeState Evaluate(){  
        if(!item || !ai.itemToPickUpCache){
            ai.moveDirection = Vector3.zero;
            return NodeState.Success; 
        }
        if(!item.CanPickup(inv.transform)){ return NodeState.Fail; } // If not in range to be picked up...
       
        ai.PickUpItem(item);
        ai.itemToPickUpCache = null;
        if(item.GetType() == typeof(Inventory_WeaponStats)){ ai.EquippedItem = item.GetItem(); }
        return NodeState.Success;
    }
}
