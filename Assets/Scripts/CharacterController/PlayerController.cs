using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.ItemStorage;
using System.Linq;
using UnityEngine.UI;
using TMPro;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.CharacterController{
    public class PlayerController : CharacterBase{
        Vector2 RAW_INPUT => new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));        
        [SerializeField] Stats playerStats;
        [Tooltip("Smaller this value, the more responsive movement will be. Greater the value, slower it is to accelerate")][SerializeField] float positionSmoothingValue;
        Camera cam;
        CameraShake camShake;
        [SerializeField] ScreenBlood screenBlood;
        [SerializeField] Transform itemPopupHolder;
        [SerializeField] GameObject itemSlot;
        Dictionary<Inventory_ItemStats, UI_FadeAndDestroy> lootableItems = new Dictionary<Inventory_ItemStats, UI_FadeAndDestroy>(); // Holds lootable items and shows a UI element if it can be picked up.
        private void Awake() {
            profile = playerStats;
            cam = Camera.main;
            camShake = cam.GetComponent<CameraShake>();
        }

        private void FixedUpdate() {            
            Move(RAW_INPUT.normalized, profile.speed, positionSmoothingValue);   
        }

        

        protected override void Update() {
            base.Update();

            #region Lootable item checking
            // Check radius for pickable items
            Physics2D.OverlapCircleAll(transform.position, 1).ToList().ForEach((item) => {
                Inventory_ItemStats invItem = item.GetComponent<Inventory_ItemStats>();
                bool CanPickup = (invItem != null)? invItem.CanPickup(transform) : false;
                if(CanPickup && !lootableItems.ContainsKey(invItem)){ 
                    GameObject slot = Instantiate(itemSlot, cam.WorldToScreenPoint(invItem.transform.position), Quaternion.identity, itemPopupHolder);                    
                    slot.transform.GetChild(0).GetComponent<Image>().sprite = invItem.GetItem().itemImage;
                    UI_FadeAndDestroy uiSlot = slot.GetComponent<UI_FadeAndDestroy>();
                    uiSlot.targetObj = invItem;
                    lootableItems.Add(invItem, uiSlot); 
                }
            });

            // If character is out of range...
            for(int i = 0; i < lootableItems.Count; i++){
                Inventory_ItemStats item = lootableItems.Keys.ToList()[i];
                if(!item){ continue; }
                
                if(!item.CanPickup(transform)){
                    lootableItems[item].DoDestroy = true;
                    lootableItems.Remove(item);
                }
                else{ lootableItems[item].DoDestroy = false; }
            }
            #endregion

            if(Input.GetKeyDown(KeyCode.E) && lootableItems.Count >0){
                List<Inventory_ItemStats> items = lootableItems.Keys.OrderBy((item) => Vector3.Distance(lootableItems[item].transform.position, transform.position)).ToList();
                PickUpItem(items[0]);
                lootableItems[items[0]].DoDestroy = true;
                lootableItems.Remove(items[0]);
            }

            if(Input.GetMouseButton(0)){                 
                UseHeldObject(equippedItemVisual);  
            }    
        }

        float Remap(float inputValue, float fromMin, float fromMax, float toMin, float toMax){
            float i = (((inputValue - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin);
            i = Mathf.Clamp(i, toMin, toMax);
            return i;
        }

        /// <summary>
        /// This function negates health off the character
        /// </summary>
        /// <param name="damage">The damage to be inflicted on this character</param>
        public override void Damage(float damage, Vector3? hittersPosition = null){ 
            base.Damage(damage, hittersPosition);
            profile.health = Mathf.Clamp(profile.health - damage, 0, profile.maxHealth); 

            camShake.Shake();
            screenBlood.UpdateFX(profile.health / profile.maxHealth);
            MusicHandler.instance.sources.ToList().ForEach((source) => {
                source.pitch = Remap(profile.health / profile.maxHealth, 0, 1, 0.9f, 1); 
            });
        }

        public override void GetHealth(out float _health, out float _maxHealth){
            _health = profile.health;
            _maxHealth = profile.maxHealth;
        }
    }
}