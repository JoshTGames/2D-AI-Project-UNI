using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.ItemStorage;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTSlotsAvailable : BTNode{
    Inventory inv;
    
    public BTSlotsAvailable(Inventory _inv){ 
        this.inv = _inv;
    }   
    
    public override NodeState Evaluate(){      
        return (inv.GetData(inv.tag).Count < inv.Slots)? NodeState.Success : NodeState.Fail;
    }
}
