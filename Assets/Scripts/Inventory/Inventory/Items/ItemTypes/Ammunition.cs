using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    [CreateAssetMenu(fileName = "New Ammunition", menuName = "ScriptableObjects/Inventory/Create Ammunition")]
    public class Ammunition : Item{       
        [SerializeField] AmmunitionData itemSettings;
        
        
        private void OnEnable() {
            itemType = ItemType.Ammunition;
            ammunitionData = itemSettings;
        }
    }
}