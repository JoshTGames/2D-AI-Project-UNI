using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.ItemStorage;
using System.Linq;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/
// DISCLAIMER: NEEDS TO USE FISTS IF THERE IS NOTHING TO USE
public class BTSelectWeapon : BTNode{
    AIBrain aI;
    Inventory inv;   
    Transform target;

    public BTSelectWeapon(AIBrain _ai, Inventory _inv, Transform _target = null){ 
        this.aI = _ai;
        this.inv = _inv;        
        this.target = _target;
    }
    
    public override NodeState Evaluate(){     
        if(!target){ return NodeState.Fail; }
        Dictionary<int, List<Item>> inventoryData = inv.GetData(aI.gameObject.tag);
        List<int> ammunitionIndexes = inv.GetItemIndexesOfType(Item.ItemType.Ammunition, aI.gameObject.tag);        

        Inventory_WeaponStats equippedItemVisual = (Inventory_WeaponStats)aI.GetEquippedItemVisual();        

        // Scan through ai selected items and choose a weapon best suited to the distance between both targets
        List<int> weapons = aI.potentialSelectableItemsFromInventory.OrderByDescending((index) => inventoryData[index][0].weaponData.sightRange).ToList();
        Weapon suitableWeapon = (weapons.Count >0)? (Weapon) inventoryData[0][0] : null; // Sets a default weapon (Chooses weapon with smallest reach)         
        for(int i = 0; i < weapons.Count; i++){
            Weapon weapon = (Weapon) inventoryData[weapons[i]][0]; // Current iterated weapon            
            float distToTarget = Vector3.Distance(target.position, aI.GetEquippedItemVisual().transform.position); // Get distance between target and this character
            float weaponRange = Mathf.Min(weapon.weaponData.sightRange, aI.vision.sightRange); // Clamp the range to whatever is smallest
            float suitableWeaponRange = Mathf.Min(suitableWeapon.weaponData.sightRange, aI.vision.sightRange); // Clamp the range to whatever is smallest

            bool thisWeaponIsBetterSuited = (distToTarget <= weaponRange);
            bool requiresAmmo = (weapon.weaponData.projectile != null); // If true, then this weapon requires ammo...
            bool hasAmmo = false; // Check if weapon has ammo for weapon
            
            bool doesSuitableWeaponRequireAmmo = (suitableWeapon.weaponData.projectile != null);
            bool doesSuitableWeaponHaveAmmo = false;
            foreach(int ammo in ammunitionIndexes){ // Iterates through each inventory index to check to see if this character has ammunition for the weapon
                if(inventoryData[ammo][0].name == weapon.weaponData.projectile?.name || equippedItemVisual.magazine >0){ hasAmmo = true; }    
                if(inventoryData[ammo][0].name == suitableWeapon.weaponData.projectile?.name){ doesSuitableWeaponHaveAmmo = true; }            
            }
            if(equippedItemVisual.magazine >0 && !doesSuitableWeaponHaveAmmo){ doesSuitableWeaponHaveAmmo = true; }
            if((thisWeaponIsBetterSuited && ((requiresAmmo && hasAmmo) || !requiresAmmo)) || (!doesSuitableWeaponHaveAmmo && doesSuitableWeaponRequireAmmo)){ 
                suitableWeapon = weapon; 
            }
        }        
        if(aI.EquippedItem?.name != suitableWeapon?.name){            
            aI.EquippedItem = suitableWeapon; // Force equips this weapon to the character
        }
        
        return NodeState.Success;
    }
}
