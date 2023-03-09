using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    public class Inventory_SlotSelectedAnimation : MonoBehaviour{
        
        public bool isSelected, isHoveredOver;
        Image image;

        [SerializeField] float smoothingSpeed;

        float fillAmntSmoothVelocity, slotScaleAmntSmoothVelocity;

        private void Awake() => image = (image)? image: GetComponent<Image>();
        void Update(){
            // OPACITY
            float fillAmntTarget = (isHoveredOver)? .5f : 0;
            fillAmntTarget = (isSelected)? 1 : fillAmntTarget;

            float fillAmount = Mathf.SmoothDamp(image.color.a, fillAmntTarget, ref fillAmntSmoothVelocity, smoothingSpeed);
            image.color = new Color(image.color.r, image.color.g, image.color.b, fillAmount);

            // SCALE
            float slotScaleTarget = (isHoveredOver)? 1.05f : 1;
            slotScaleTarget = (isSelected)? 1.1f : slotScaleTarget;
            float interpolatedScale = Mathf.SmoothDamp(transform.parent.localScale.x, slotScaleTarget, ref slotScaleAmntSmoothVelocity, smoothingSpeed);            
            transform.parent.localScale = new Vector3(interpolatedScale, interpolatedScale, 1);
        }
    }
}