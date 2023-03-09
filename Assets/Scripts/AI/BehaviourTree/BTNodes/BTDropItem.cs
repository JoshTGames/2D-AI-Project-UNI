using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.ItemStorage;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTDropItem : BTNode{
    AIBrain ai;
    Inventory inv;
    Inventory_ItemStats itemObj;
    
    public BTDropItem(AIBrain _ai, Inventory _inv, Inventory_ItemStats _item){ 
        this.ai = _ai;
        this.inv = _inv;
        this.itemObj = _item;
    }   

    NodeState taskState;

    public override NodeState Evaluate(){
        if(!ai.itemToPickUpCache){ return NodeState.Success; }
        if(!itemObj || Vector3.Distance(ai.transform.position, itemObj.transform.position) > itemObj.itemSettings.interactRadius){ return NodeState.Fail; }        
        if(ai.itemToDropIndexCache == null){ return NodeState.Fail; }

        Dictionary<int, List<Item>> invData = inv.GetData(inv.tag);
        ai.DropItem(invData[(int)ai.itemToDropIndexCache][0].itemPrefab, invData[(int)ai.itemToDropIndexCache][0].maxStack, ai.itemToDropIndexCache);        
        ai.itemToDropIndexCache = null;
        return NodeState.Success;
    }
}
