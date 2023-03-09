using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JoshGames.CharacterController;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    public class Inventory_Hotbar : MonoBehaviour{
        [SerializeField] Inventory hotbar;
        [SerializeField] PlayerController player;
        [SerializeField] int allocatedSlots;

        public Inventory Hotbar{
            get{ return hotbar; }
            private set{ 
                hotbar = value; 
                //UpdateInventory();
            }
        }

        List<int> slotsInherited; // This holds all the indexes from the inventory which this hotbar holds

        [SerializeField] GameObject slot;

        int selectedSlot = 0; // ARRAY BASED SYSTEM 0 BEING THE FIRST VALUE...
        int SelectedSlot{
            get{ return selectedSlot; }
            set{                
                value = Mathf.Clamp(value, 0, slots.Count-1);
                if(selectedSlot != value){    
                    Dictionary<int, List<Item>> inventory = hotbar.GetData(gameObject.tag); // THIS WILL HOLD THE LAST AVAILABLE INVENTORY                
                    player.EquippedItem = (inventory.ContainsKey(slotsInherited[value]))? inventory[slotsInherited[value]][0] : hotbar.GetUnhandedItem();
                    selectedSlot = value;
                }                
            }
        }
        List<Inventory_SlotSelectedAnimation> slots = new List<Inventory_SlotSelectedAnimation>();
        
        /// <summary>
        /// Every time this function is called, It will update the slot corrosponding to the id it holds
        /// </summary>
        /// <param name="index">The slot we are updating</param>
        void SlotHasItem(int index){
            Dictionary<int, List<Item>> inventory = hotbar.GetData(gameObject.tag); // THIS WILL HOLD THE LAST AVAILABLE INVENTORY

            Transform slot = slots[index].transform.parent;
            GameObject item = slot.GetChild(1).gameObject;
            GameObject quantity = slot.GetChild(2).gameObject;

            bool inventoryHasSlot = inventory.ContainsKey(slotsInherited[index]) && inventory[slotsInherited[index]][0].name != "Fists"; // If inventory slot is holding and item which isn't Fists...
            bool itemIsStackable = inventoryHasSlot && inventory[slotsInherited[index]][0].maxStack > 1;

            item.SetActive(inventoryHasSlot);
            quantity.SetActive((itemIsStackable)? true: false); // IF IT IS A STACKABLE ITEM, THEN IT'LL SHOW A QUANTITY OF THE ITEM

            if(inventoryHasSlot){
                item.GetComponent<Image>().sprite = inventory[slotsInherited[index]][0].itemImage;
                quantity.GetComponent<TextMeshProUGUI>().text = $"x{inventory[slotsInherited[index]].Count}";
            }
        }

        /// <summary>
        /// This function creates the number of slots allocated to this interface by the inventory
        /// </summary>
        void CreateSlots(){
            int amntOfSlotsMade = slots.Count;
            for(int i = amntOfSlotsMade; i < slotsInherited.Count; i++){
                GameObject newSlot = Instantiate(slot, transform);
                slots.Add(newSlot.transform.GetChild(0).GetComponent<Inventory_SlotSelectedAnimation>());          
                newSlot.GetComponent<Inventory_SlotHover>().slotId = i;   
                
                SlotHasItem(i);
            }
        }

        /// <summary>
        /// Initialisation of this script
        /// </summary>
        private void Awake() {            
            slotsInherited = hotbar.DeligateSlots(allocatedSlots); // Get slot indexes to work for this hotbar
            CreateSlots();
        }

        /// <summary>
        /// This function will change which slot is selected based on various inputs
        /// </summary>
        private void Update() {                                          
            for(int i = 0; i < slots.Count; i++){ // KEY NUMERIC SLOT SWITCHING
                if(Input.GetKeyDown($"{(i+1) % 10}")){ SelectedSlot = i; }

                slots[i].isSelected = (i != SelectedSlot)? false: true;
                SlotHasItem(i);
            }
            SelectedSlot += Mathf.RoundToInt(Input.mouseScrollDelta.y); // MOUSE SCROLL WHEEL SLOT SWITCHING  
            SelectedSlot += (Input.GetButtonDown("ControllerHotbarSwitchRight"))? 1 : 0; // RIGHT BUMPER OF A CONTROLLER
            SelectedSlot -= (Input.GetButtonDown("ControllerHotbarSwitchLeft"))? 1 : 0; // LEFT BUMPER OF A CONTROLLER

            if(player.updateHeldSlot){
                Dictionary<int, List<Item>> inventory = hotbar.GetData(gameObject.tag); // THIS WILL HOLD THE LAST AVAILABLE INVENTORY                
                player.EquippedItem = (inventory.ContainsKey(slotsInherited[SelectedSlot]))? inventory[slotsInherited[SelectedSlot]][0] : hotbar.GetUnhandedItem();
                player.updateHeldSlot = false;
            }

            if(Input.GetKeyDown(KeyCode.Q)){
                Dictionary<int, List<Item>> inventory = hotbar.GetData(gameObject.tag); // THIS WILL HOLD THE LAST AVAILABLE INVENTORY
                if(!inventory.ContainsKey(slotsInherited[SelectedSlot])){ return; }
                player.DropItem(player.GetEquippedItemVisual().GetItem().itemPrefab, 1, slotsInherited[SelectedSlot]);
            }
        }

        public void OnSlotClick(int slotId) => SelectedSlot = slotId;
    }
}