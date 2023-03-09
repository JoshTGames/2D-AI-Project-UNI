using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class CameraShake : MonoBehaviour{
    Camera cam;
    
    [SerializeField] float shakeTime;
    [SerializeField] AnimationCurve smoothing;
    bool performShake;
    public float beforeShake;
    [SerializeField] float shakeMultiplier;

    float curTime;
    public void Shake(){
        performShake = true;
        curTime = 0;
    }

    float Remap(float inputValue, float fromMin, float fromMax, float toMin, float toMax){
        float i = (((inputValue - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin);
        i = Mathf.Clamp(i, toMin, toMax);
        return i;
    }

    private void Awake(){ 
        cam = Camera.main; 
        beforeShake = cam.orthographicSize;
    }
    void FixedUpdate(){        
        curTime = Mathf.Clamp(curTime + ((performShake)? Time.fixedDeltaTime : -Time.fixedDeltaTime), 0, shakeTime/2);        

        if(((shakeTime/2) - curTime) <= 0){ performShake = false; }

        float step = Mathf.Clamp01(curTime / (shakeTime/2));
        float value = Remap(1 - smoothing.Evaluate(step), 0, 1, beforeShake * shakeMultiplier, beforeShake);
        cam.orthographicSize = value;
    }
}
