using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class ScreenBlood : MonoBehaviour{
    Image bloodFX;

    private void Awake() => bloodFX = GetComponent<Image>();


    public void UpdateFX(float alpha = 0){
        bloodFX.color = new Color(bloodFX.color.r, bloodFX.color.g, bloodFX.color.b, 1 - alpha);
    }
}
