using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JoshGames.CharacterController;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class UI_Health : MonoBehaviour{    
    [SerializeField] Image thisImg;
    public CharacterBase character;
    [SerializeField] float updateFrequency;

    [SerializeField] bool doHideAfterTime;
    [SerializeField] float timeTillHide; // x amount of seconds until the parent obj and its children will be hidden...
    float hidingTime;
    [SerializeField] float smoothingLatency, changeStateSpeed;

    
    float smoothingRef;
    bool hideUI = true;

    int healthImgIndex;

    [SerializeField] List<ImageData> images = new List<ImageData>();
    [System.Serializable] public class ImageData{
        public Image image;
        [HideInInspector] public Color targetColour;
        [HideInInspector] public float defaultAlpha;

        public ImageData(Image _image, float _defaultAlpha){
            this.image = _image;
            this.defaultAlpha = _defaultAlpha;
        }
    }
    public Color safeColour, dangerColour;    
    [HideInInspector] public float targetValue; // This is the value the image fill will want to be at...

    private void Awake() {
        thisImg = (thisImg)? thisImg : GetComponent<Image>();

        InvokeRepeating("GetHealth", 0f, updateFrequency);
        
        for(int i = 0; i < images.Count; i++){            
            if(images[i].image.GetInstanceID() == thisImg.GetInstanceID()){ healthImgIndex = i; }
            images[i].defaultAlpha = images[i].image.color.a * 255;            
            if(doHideAfterTime){
                images[i].targetColour = images[i].image.color * new Color(1, 1, 1, 0); 
            }
            else{
                images[i].targetColour = images[i].image.color;
                ShowUI();
            }
        }
    }


    /// <summary>
    /// Every x seconds, this function will be called and update the float which will then be interpolated on the image
    /// </summary>
    void GetHealth(){
        float health, maxHealth;
        character.GetHealth(out health, out maxHealth);

        targetValue = (health / maxHealth);        
    }

    void ShowUI(){
        foreach(ImageData img in images){
            // Set target to default               
            img.targetColour = img.targetColour + new Color(0, 0, 0, img.defaultAlpha);                  
        }
    }
    void HideUI(){
        foreach(ImageData img in images){
            // Set target to transparent                
            img.targetColour = img.targetColour * new Color(1, 1, 1, 0);
        } 
    }

    private void Update() {
        thisImg.fillAmount = Mathf.SmoothDamp(thisImg.fillAmount, targetValue, ref smoothingRef, smoothingLatency);        
        images[healthImgIndex].targetColour = Color.Lerp(dangerColour, safeColour, targetValue);

        
        if(targetValue >= 1 && !hideUI){
            hideUI = true;
            hidingTime = Time.time + timeTillHide;
        }
        else if(hideUI && targetValue < 1){            
            hideUI = false;
            ShowUI();
        }


        if(doHideAfterTime && ((Time.time >= hidingTime && hideUI) || targetValue <= 0)){ HideUI(); }

        foreach(ImageData img in images){
            img.image.color = Color.Lerp(img.image.color, img.targetColour, Time.deltaTime * changeStateSpeed);
        }
    }
}
