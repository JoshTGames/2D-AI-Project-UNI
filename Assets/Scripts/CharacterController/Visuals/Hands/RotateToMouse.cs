using UnityEngine;
using System;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class RotateToMouse : MonoBehaviour{
    [Serializable] public struct HandSettings{
        public Transform obj;
        public float offsetRotationAngle;        
        public bool invertY;        
        public bool teleportRotationOnParentScaleChange;
        [Range(0, 1)] public float smoothing;
        [HideInInspector] public float smoothVelocity;
        [HideInInspector] public float objSmoothing, smoothingOverride;
        [HideInInspector] public float overrideAngle;
        [HideInInspector] public Vector2 lastParentRotation;
    }

    [SerializeField] Transform parentObj;
    public HandSettings[] objectsToRotate;    
    [SerializeField] Transform target;
    [SerializeField] AIBrain aI;
    

    
    
    Camera cam;
    private void Awake() {
        cam = Camera.main;        
    }

    Quaternion GetRotation(Vector3 targetPos, int i){
        Vector3 dir = targetPos - parentObj.position;
        Debug.DrawRay(transform.position, dir);
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + objectsToRotate[i].offsetRotationAngle;

        objectsToRotate[i].obj.localScale = new Vector3(
            (parentObj.localScale.x > 0)? 1 : -1, 
            ((parentObj.localScale.x > 0 && objectsToRotate[i].invertY) || (parentObj.localScale.x < 0 && !objectsToRotate[i].invertY))? 1: -1
            , 1
        );
        
        objectsToRotate[i].obj.localScale += new Vector3(0, (!objectsToRotate[i].invertY)? 2: 0, 0);

        bool hasChanged = false;
        if((Vector2)parentObj.localScale != objectsToRotate[i].lastParentRotation && objectsToRotate[i].teleportRotationOnParentScaleChange){
            hasChanged = true;            
            objectsToRotate[i].lastParentRotation = parentObj.localScale;                       
        }

        objectsToRotate[i].objSmoothing = objectsToRotate[i].smoothing;
        if(objectsToRotate[i].overrideAngle != 0){ 
            angle += (objectsToRotate[i].overrideAngle * -parentObj.localScale.x);
            objectsToRotate[i].objSmoothing = objectsToRotate[i].smoothingOverride;             
        }        
        angle = Mathf.SmoothDampAngle(objectsToRotate[i].obj.rotation.eulerAngles.z, angle, ref objectsToRotate[i].smoothVelocity, (!hasChanged)? objectsToRotate[i].objSmoothing : 0);
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }  

    private void FixedUpdate() {        
        Vector3 targetPos = cam.ScreenToWorldPoint(Input.mousePosition);  
        if(aI && !target){
            targetPos = aI.positionOfInterest;
        }
        for(int i = 0; i< objectsToRotate.Length; i++){
            Transform obj = objectsToRotate[i].obj;            
            obj.rotation = GetRotation((target)? target.position: targetPos, i);                 
        }
    }
}
