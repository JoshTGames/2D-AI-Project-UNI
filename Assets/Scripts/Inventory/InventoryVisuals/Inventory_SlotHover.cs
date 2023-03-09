using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    public class Inventory_SlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{
        Inventory_SlotSelectedAnimation slot;
        Inventory_Hotbar hotbar;
        public int slotId;

        private void Awake(){
            slot = (slot)? slot : transform.GetChild(0).GetComponent<Inventory_SlotSelectedAnimation>(); 
            hotbar = (hotbar)? hotbar : transform.parent.GetComponent<Inventory_Hotbar>();
        }      
        

        public void OnPointerEnter(PointerEventData eventData) => slot.isHoveredOver = true;    
        public void OnPointerExit(PointerEventData eventData) => slot.isHoveredOver = false;  
        public void OnPointerClick(PointerEventData eventData) => hotbar.OnSlotClick(slotId);    
    }
}