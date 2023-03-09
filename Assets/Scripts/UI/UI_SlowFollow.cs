using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoshGames.CharacterController;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class UI_SlowFollow : MonoBehaviour{    
    [SerializeField] ImageData thisImg = new ImageData(null, 0);
    [SerializeField] UI_Health health; 
    [SerializeField] float smoothingLatency;    
    float smoothingRef;
   
    
    [System.Serializable] public class ImageData{
        public Image image;
        [HideInInspector] public Color targetColour;
        [HideInInspector] public float defaultAlpha;

        public ImageData(Image _image, float _defaultAlpha){
            this.image = _image;
            this.defaultAlpha = _defaultAlpha;
        }
    }    

    private void Awake() {
        thisImg.image = (thisImg.image)? thisImg.image : GetComponent<Image>();
    }

    private void Update() {
        thisImg.image.fillAmount = Mathf.SmoothDamp(thisImg.image.fillAmount, health.targetValue, ref smoothingRef, smoothingLatency);        
    }
}
