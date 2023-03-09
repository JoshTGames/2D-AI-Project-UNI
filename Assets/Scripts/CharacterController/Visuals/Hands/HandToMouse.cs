using UnityEngine;
using System;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class HandToMouse : MonoBehaviour{
    [Serializable] public struct HandSettings{
        public Transform handObj;
        [HideInInspector] public Vector3 offsetPos;
        [HideInInspector] public float targetPositionMultiplier;
        [HideInInspector] public Vector3 smoothVelocity;
        [HideInInspector] public float objSmoothing, smoothingOverride;
    }
    [SerializeField] Transform parentObj;
    [SerializeField] Transform target;
    [SerializeField] AIBrain aI;
    [SerializeField] float minDistanceFromBody = .1f, maxDistanceFromBody = .5f;
    [SerializeField] float offsetMultiplier;

    [SerializeField] [Range(0, 1)] float smoothing;
    
    public HandSettings[] hands;
    Camera cam;

    public Vector3 ClampMagnitude(Vector3 v, float min, float max){
        float sm = v.sqrMagnitude;        
        if(sm > max * max){ return v.normalized * max; }
        else if(sm < min * min){ return v.normalized * min; }
        
        return v;
    }
   
    float hasChanged; // IF THIS DOESNT = THE SAME AS THE PARENT OBJECT THEN IT WILL FORCE THE HAND POSITION TO A POSITION TO ENSURE SEAMLESSNESS
    Vector3 GetPosition(Vector3 targetPos, int i){  
        targetPos.z = -10;      
        Transform hand = hands[i].handObj;
        Vector3 dir = targetPos - parentObj.position;      
        Vector3 offsetPos = (parentObj.localScale.x > 0) ? hands[i].offsetPos : new Vector3(-hands[i].offsetPos.x, hands[i].offsetPos.y, hands[i].offsetPos.z);

        Vector3 clampedDistance = ClampMagnitude((offsetPos * offsetMultiplier) + dir.normalized, minDistanceFromBody, maxDistanceFromBody);
        Vector3 targetPosition = parentObj.position + clampedDistance;        

        hands[i].objSmoothing = smoothing;
        if(hands[i].targetPositionMultiplier != 0){     
            dir = new Vector3(targetPos.x, targetPos.y, 0) - parentObj.position;  
            targetPosition = parentObj.position + (dir.normalized * hands[i].targetPositionMultiplier); 
            hands[i].objSmoothing = hands[i].smoothingOverride;
        }      
        targetPosition.z = 0;
               
        return (parentObj.localScale.x != hasChanged)? targetPosition : Vector3.SmoothDamp(hand.position, targetPosition, ref hands[i].smoothVelocity, hands[i].objSmoothing);  
    }
    

    private void Awake() {
        cam = Camera.main;
        for(int i = 0; i< hands.Length; i++){
            hands[i].offsetPos = hands[i].handObj.localPosition;
        }
    }


    public Transform GetPrimaryHand() => hands[0].handObj;   

    private void FixedUpdate() {
        Vector3 targetPos = cam.ScreenToWorldPoint(Input.mousePosition);  
        if(aI && !target){ targetPos = aI.positionOfInterest; }
        for(int i = 0; i< hands.Length; i++){
            Transform hand = hands[i].handObj;            
            // Check character for weapon... If weapon is found and has a hand position
            hand.position = GetPosition((target)? target.position: targetPos, i); 
            hasChanged = parentObj.localScale.x;      
        }
    }
}
