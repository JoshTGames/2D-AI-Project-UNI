using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    [CreateAssetMenu(fileName = "New Resource", menuName = "ScriptableObjects/Inventory/Create Resource")]
    public class Resource : Item{       
        [SerializeField] ResourceData itemSettings;
        
        
        private void OnEnable() {
            itemType = ItemType.Weapon;
            resourceData = itemSettings;
        }
    }
}