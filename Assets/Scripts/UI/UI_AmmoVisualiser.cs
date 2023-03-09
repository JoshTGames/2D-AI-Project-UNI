using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoshGames.CharacterController;
using JoshGames.ItemStorage;
using TMPro;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class UI_AmmoVisualiser : MonoBehaviour{    
    [SerializeField] CharacterBase character;
    [SerializeField] float updateFrequency;

    [SerializeField] TextMeshProUGUI roundsUI, clipsUI;
    [SerializeField] Image ammoImage;

    private void Awake() => InvokeRepeating("UpdateState", 0f, updateFrequency);


    /// <summary>
    /// Every x seconds, this function will be called and update the texts
    /// </summary>
    void UpdateState(){
        Inventory_WeaponStats weapon = null;
        Inventory_ItemStats equippedItem = character.GetEquippedItemVisual();
        if(equippedItem?.GetType() == typeof(Inventory_WeaponStats)){ weapon = (Inventory_WeaponStats) equippedItem; }
        if(weapon && !weapon.GetItem().weaponData.projectile){ weapon = null; } // If not ranged item

        roundsUI.gameObject.SetActive(weapon);
        clipsUI.gameObject.SetActive(weapon);
        ammoImage.gameObject.SetActive(weapon);
        if(!weapon){ return; }

        roundsUI.text = $"Rounds: {weapon.magazine}/{weapon.GetItem().weaponData.magCapacity}";
        roundsUI.text = (weapon.isReloading)? "RELOADING..." : roundsUI.text;
        Dictionary<int, List<Item>> invData = character.inventory.GetData(character.gameObject.tag);
        List<int> ammunitionIndexes = character.inventory.GetItemIndexesOfType(Item.ItemType.Ammunition, character.gameObject.tag);        

        int numberOfAmmo = 0;
        foreach(int index in ammunitionIndexes){
            if(invData[index][0].name != weapon.GetItem().weaponData.projectile.name){ continue; }
            numberOfAmmo += invData[index].Count;
        }
        clipsUI.text = $"{Mathf.CeilToInt((float)numberOfAmmo / (float)weapon.GetItem().weaponData.magCapacity)}";
        ammoImage.preserveAspect = true;
        ammoImage.sprite = weapon.GetItem().weaponData.projectile.itemImage;
    }
}
