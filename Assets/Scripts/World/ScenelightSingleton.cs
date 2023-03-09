using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

[RequireComponent(typeof(Light2D))]
public class ScenelightSingleton : MonoBehaviour{
    public static ScenelightSingleton instance;
    [SerializeField] Light2D sceneLight;

    public Light2D SceneLight{
        get{ return sceneLight; }
    }
    
    void Awake(){
        if(instance){
            Destroy(this);
            return;
        }
        instance = this;

        sceneLight = (sceneLight)? sceneLight : GetComponent<Light2D>();
    }    
}
