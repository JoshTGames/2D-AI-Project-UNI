using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    public class Inventory_ResourceStats : Inventory_ItemStats{
        [SerializeField] Resource thisItem;
        private void OnEnable() => item = thisItem;
    }
}