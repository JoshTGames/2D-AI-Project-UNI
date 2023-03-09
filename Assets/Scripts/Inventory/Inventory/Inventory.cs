using System.Collections.Generic;
using UnityEngine;
using System;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    public class Inventory : MonoBehaviour{
        [Serializable] public struct InventorySettings{
            [Tooltip("The size of the inventory")] public int slots;
            [Tooltip("Any type of item added here will not be cachable by the holder")] public Item.ItemType[] exemptItemTypes; // ANYTHING IN THIS ARRAY CANNOT BE PICKED UP BY THIS OBJECT SCRIPT
            [Tooltip("Any gameobject with a tag equal to one in this array will be able to access the inventory this holder possesses")] public string[] accessTags;

            [Tooltip("This item to hold when the inventory slot holds nothing")] public Item unhandedItem;
        }
        [SerializeField] InventorySettings inventorySettings;
        public InventorySettings Settings{
            get{ return inventorySettings; }
            private set{ inventorySettings = value; }
        }

        public int Slots{
            get{ return inventorySettings.slots; }
        }
        
        [HideInInspector] public Item equippedItem = null; // This is the item that will be visually equipped in the hands of the character
        public Item GetUnhandedItem() => inventorySettings.unhandedItem;
        
        HashSet<int> usedSlots = new HashSet<int>(); // FOR UI stuff, e.g. Seperating the Hotbar, from the core inventory...
        public List<int> DeligateSlots(int reserveAmount){
            List<int> reservedSlots = new List<int>();
            for(int i = 0; i < inventorySettings.slots; i++){
                if(usedSlots.Contains(i)){ continue; }
                if(reservedSlots.Count >= reserveAmount){ break; }

                usedSlots.Add(i);
                reservedSlots.Add(i);
            }           
            return reservedSlots;
        }
        
        private Dictionary<int, List<Item>> inventory = new Dictionary<int, List<Item>>(); // INT IS THE SLOT NUMBER, AND THEN WE WILL HOLD A LIST OF OBJECTS, IF LIST EXCEEDS MAX STACK OF OBJECT, THEN CREATE A NEW KEY
        


        // FUNCTIONS
        // ---------

        public bool IsAccessible(string tag){ // RETURNS IF THIS STORAGE IS ACCESSIBLE OR NOT
            bool found = false;
            foreach (string curTag in Settings.accessTags){
                if(curTag == tag) { 
                    found = true; 
                    break; 
                }
            }
            return (found || Settings.accessTags.Length == 0) ? true : false;
        }
        public Dictionary<int, List<Item>> GetData(string tag) => (IsAccessible(tag)) ? inventory: null; // RETURNS THE DATA BACK TO THE CALLING SCRIPT IF THEY HAVE PERMISSION TO READ IT

        private bool CanAcceptItem(Item.ItemType itemType){ // THIS MAKES SURE ONLY ITEMS OF X TYPE CAN BE ADDED TO INVENTORY                
            foreach (Item.ItemType item in Settings.exemptItemTypes){
                if (itemType == item) { return false; }
            }
            return true;
        } 

        public int Add(string tag, int slotIndex, Item obj, int amt = 1){ // ADDS OBJS TO SLOT
            if(!IsAccessible(tag)){ return 0; }
            if(!CanAcceptItem(obj.itemType)){ return 0; } // STOPS ANY ITEMS BEING ADDED WITHOUT PERMISSION
            if(!inventory.ContainsKey(slotIndex)){ inventory.Add(slotIndex, new List<Item>()); } // Makes the list ready for appending items to
            int fillAmt = 0;
            
            if(inventory[slotIndex].Count >0 && inventory[slotIndex][0].name != obj.name){ return amt; } // Stops items being added to a slot which isnt of that type
            if (inventory[slotIndex].Count + amt > obj.maxStack){ // IF OVERFLOWING WITH NEW AMT...            
                fillAmt = obj.maxStack - inventory[slotIndex].Count;  // FILL UP SLOT TO MAX...               
            }
            else{ fillAmt = amt; } // THIS MEANS THAT THERE IS NO OVERFLOW

            for (int x = 0; x < fillAmt; x++) { inventory[slotIndex].Add(obj); }
            return amt -= fillAmt; // RETURNS THE EXCESS; IF 0, NOTHING WILL HAPPEN OTHERWISE THE PARENT FUNCTION WILL TRY CREATE A NEW SLOT
        } 

        public int Remove(string tag, int slotIndex, int amt = 1){ // REMOVES OBJS FROM SLOT
            if(!IsAccessible(tag)){ return 0; }
            int amountToRemove = Mathf.Min(amt, inventory[slotIndex].Count);
            inventory[slotIndex].RemoveRange(0, amountToRemove);
            if(inventory[slotIndex].Count <= 0){ RemoveSlot(slotIndex); }
            return amountToRemove;
        } 
        private void RemoveSlot(int slotIndex){ inventory.Remove(slotIndex); } // REMOVES AN ENTIRE SLOT
        public bool Swap(string tag, int slotIndexA, int slotIndexB){ // SWAPS ITEM A WITH B IF B EXISTS (IN LOCAL INVENTORY!) -- THIS WONT WORK CROSS-INVENTORY
            if(!IsAccessible(tag)){ return false; }
            if (inventory.ContainsKey(slotIndexB)){
                List<Item> tempItem = inventory[slotIndexB];
                inventory[slotIndexB] = inventory[slotIndexA];
                inventory[slotIndexA] = tempItem;
                //UpdateVisuals();
                return true;
            }
            return false;
        } 
    
        public List<int> GetItemIndexesOfType(Item.ItemType itemType, string tag){
            if(!IsAccessible(tag)){ return null; }
            List<int> itemIndexes = new List<int>();
            foreach(int item in inventory.Keys){       
                   
                // Checks to make sure the item is of the item type and that it is not already in the selected item list...    
                if(!(inventory[item][0].itemType == itemType && !itemIndexes.Contains(item))){ continue; }
                itemIndexes.Add(item);
            }            
            return itemIndexes;
        }
    }
}