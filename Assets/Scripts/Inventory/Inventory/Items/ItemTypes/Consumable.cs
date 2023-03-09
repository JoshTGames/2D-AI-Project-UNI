using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "ScriptableObjects/Inventory/Create Consumable")]
    public class Consumable : Item{       
        [SerializeField] ConsumableData itemSettings;
        
        
        private void OnEnable() {
            itemType = ItemType.Consumable;
            consumableData = itemSettings;
        }
    }
}