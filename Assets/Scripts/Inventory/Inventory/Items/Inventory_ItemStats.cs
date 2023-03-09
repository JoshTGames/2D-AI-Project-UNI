using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    public class Inventory_ItemStats : MonoBehaviour{
        #region MAIN VARIABLES 
        [Serializable] public struct Settings{
            public float interactRadius;
            public bool isLootable; // If not owned... then it is on the floor and lootable...
        }
        public Settings itemSettings;
        public int quantityOfItem;
        #endregion
        
        
        protected Item item; // THIS WILL BE HANDLED IN OTHER INHERITING SCRIPTS
        

        public float GetRadius(){ return itemSettings.interactRadius; }   
        public Item GetItem() => item;

        public bool CanPickup(Transform characterPos){
            if(!itemSettings.isLootable){ return false; }
            if(Vector3.Distance(transform.position, characterPos.position) > itemSettings.interactRadius){ return false; }            
            return true;
        }

        protected virtual void Start() {                     
            if(item == null){ 
                Destroy(gameObject);                 
                return;
            }
            
            quantityOfItem = Mathf.Clamp(quantityOfItem, 0, item.maxStack);
            if(quantityOfItem <= 0){ Destroy(gameObject); }
        }


        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, itemSettings.interactRadius);
        }
    }
}