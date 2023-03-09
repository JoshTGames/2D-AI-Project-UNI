using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/


public class BodyToMouse : MonoBehaviour{
    Camera cam;
    [SerializeField] Transform focusPoint;
    [SerializeField] Transform target;
    [SerializeField] AIBrain aI;

    int previousDir;

    private void Awake() {
        cam = Camera.main;
    }

    private void Update() {
        Vector3 targetPos = cam.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = 0;

        if(aI && !target){
            targetPos = aI.positionOfInterest;
        }
        Vector3 newtargetPos = (target)? target.position : targetPos;        
        
        
        int dir = Mathf.FloorToInt(Mathf.Round((newtargetPos - focusPoint.position).normalized.x)); 
        dir = (dir == 0)? previousDir : dir;  
        if(dir == 0){ dir = 1; }  // If dir STILL = 0...      
        previousDir = dir;
        transform.localScale = new Vector3(dir, 1, 1);
    }    
}
