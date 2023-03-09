using System.Collections.Generic;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.ItemStorage;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTScanInventoryForItemType : BTNode{
    AIBrain aI;
    Inventory inv;
    Item.ItemType itemType;

    public BTScanInventoryForItemType(AIBrain _ai, Inventory _inv, Item.ItemType _itemType){ 
        this.aI = _ai;
        this.inv = _inv;
        this.itemType = _itemType;
    }
    
    public override NodeState Evaluate(){           
        aI.potentialSelectableItemsFromInventory = inv.GetItemIndexesOfType(itemType, aI.gameObject.tag);
        return (aI.potentialSelectableItemsFromInventory.Count > 0)? NodeState.Success : NodeState.Running; 
    }
}
