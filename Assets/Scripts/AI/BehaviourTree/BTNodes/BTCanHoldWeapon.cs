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

public class BTCanHoldWeapon : BTNode{
    AIBrain ai;
    Inventory inv;
    Inventory_ItemStats itemObj;
    bool canFail;

    public BTCanHoldWeapon(AIBrain _ai, Inventory _inv, Inventory_ItemStats _itemObj, bool _canFail = false){ 
        this.ai = _ai;
        this.inv = _inv;
        this.itemObj = _itemObj;
        this.canFail = _canFail;
    }
    
    public override NodeState Evaluate(){        
        if(!itemObj){ 
            ai.itemToPickUpCache = null;
            return NodeState.Success; 
        }
        Item item = itemObj.GetItem();
        float selectedWeaponDPS = item.weaponData.weaponDamage / (item.weaponData.fireRate + item.weaponData.timeToAttack); 

        Dictionary<int, List<Item>> invData = inv.GetData(inv.tag);
        List<int> weapons = inv.GetItemIndexesOfType(Item.ItemType.Weapon, inv.tag); 
        bool dontDrop = (weapons.Count < ai.agentProfile.maxWeaponaryThisCharacterCanHold);        
        if(dontDrop){
            ai.itemToPickUpCache = itemObj; 
            ai.itemToDropIndexCache = null;
            return NodeState.Success; 
        } // Stops dropping the weapon if it is not needed...
        
        int lowestIndex = weapons[0];
        float lowestDps = invData[lowestIndex][0].weaponData.weaponDamage / (invData[lowestIndex][0].weaponData.fireRate + invData[lowestIndex][0].weaponData.timeToAttack);
        weapons.ForEach((weapon) =>{
            Item.WeaponData weaponData = invData[weapon][0].weaponData;
            float dps = weaponData.weaponDamage / (weaponData.fireRate + weaponData.timeToAttack);
            
            if(dps <= lowestDps){
                lowestIndex = weapon;
                lowestDps = dps;
            }            
        });

        bool canDropItem = (lowestDps < selectedWeaponDPS);
        if(canDropItem){ 
            ai.itemToDropIndexCache = lowestIndex; 
            ai.itemToPickUpCache = itemObj; 
        }
        return (!canFail || canDropItem)? NodeState.Success : NodeState.Fail;
    }
}
