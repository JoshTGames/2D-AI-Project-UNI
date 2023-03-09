using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class HeadToMouse : MonoBehaviour{

    [SerializeField] Transform headObj, target;
    [SerializeField] MinMax angleLimits;
    [SerializeField] [Range(0, 1)] float smoothing;
    Vector3 smoothVelocity;
    Camera cam;


    [Serializable] public struct MinMax{
        public float min, max;
    }

    void LookAtMouse(Vector2 targetPos){
        Vector2 dir = (targetPos - (Vector2)headObj.position);
        headObj.right = dir.normalized * Mathf.Sign(transform.localScale.x);

        Vector3 angle = headObj.localEulerAngles;
        angle.z = Mathf.Clamp(
            angle.z - ((angle.z > 180)? 360 : 0)
            , angleLimits.min
            , angleLimits.max
        );
        headObj.localEulerAngles = angle;
    }


   private void Awake() => cam = Camera.main;

    private void FixedUpdate() {
        Vector3 mousepos = cam.ScreenToWorldPoint(Input.mousePosition);   
        LookAtMouse((target)? target.position: mousepos);
    }
}
