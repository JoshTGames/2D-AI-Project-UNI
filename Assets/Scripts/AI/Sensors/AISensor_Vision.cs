using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using System.Linq;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.AI.Sensors{
    [RequireComponent(typeof(Light2D))]
    public class AISensor_Vision : MonoBehaviour{
        [SerializeField] Light2D globalLighting; // THIS IS THE SCENE LIGHT, IT WILL BE THE BASE LIGHTING WHICH WILL DETERMINE FOV RANGE... Brighter the intensity, greater the range (Max of 1x intensity)
        [SerializeField] Light2D fovComponent;
        [SerializeField] MinMax fovRange; // THIS IS THE RANGE OF THE FOV
        [SerializeField] float visionAngle; // THE VISION ANGLE
        [SerializeField] float delayCheck; // CHECKS PER SECOND (FOR DYNAMIC LIGHTING CHECKING)
        [SerializeField] bool showDebug; // IF TRUE, WILL SHOW INFORMATION ABOUT WHATS INSIDE THE CONE
        [SerializeField] Color debugColor; // FOR DEBUGGING - Colour of all information inside vision
        float debugCheckRadius; // Updated in real-time, will show how large the search radius is for collisions...
        HashSet<Transform> selectedTargets = new HashSet<Transform>(); // FOR DEBUGGING - Target to be selected inside vision

        [HideInInspector] public float sightRange;
       
        [Serializable] public struct MinMax{ public float min, max; }

        /// <summary>
        /// This function is similar to 'Mathf.Clamp()'. However, will convert the input values proportionally to the output values...
        /// </summary>
        /// <param name="inputValue">The live value which needs to be converted</param>
        /// <param name="fromMin">The minimum value the live value could possibly be</param>
        /// <param name="fromMax">The maxium value the lvie value could possibly be</param>
        /// <param name="toMin">The new minimum value</param>
        /// <param name="toMax">The new maximum value</param>
        /// <returns></returns>
        float Remap(float inputValue, float fromMin, float fromMax, float toMin, float toMax){
            float i = (((inputValue - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin);
            i = Mathf.Clamp(i, toMin, toMax);
            return i;
        }
        
        /// <summary>
        /// Assigns variables
        /// </summary>
        private void Awake() {
            fovComponent = (fovComponent) ? fovComponent : GetComponent<Light2D>();    
            fovComponent.pointLightOuterRadius = fovRange.max;                  
            fovComponent.pointLightOuterAngle = visionAngle;                 
            fovComponent.pointLightInnerAngle = visionAngle;                 
        }        

        /// <summary>
        /// Assigns variables and 
        /// </summary>
        private void Start() {
            globalLighting = (globalLighting) ? globalLighting : ScenelightSingleton.instance.SceneLight;
            InvokeRepeating("UpdateFOVSensitivity", 0, delayCheck);
        }

        /// <summary>
        /// This function updates the range of the FOV based on the global lighting... TODO: Also consider, near-by lighting 
        /// </summary>
        void UpdateFOVSensitivity(){
            float distance = Remap(globalLighting.intensity, 0, 1, fovRange.min, fovRange.max);
            fovComponent.pointLightOuterRadius = distance;
            fovComponent.pointLightInnerRadius = distance;            
            sightRange = distance;
        }
        

        /// <summary>
        /// This function will return true/false if a given Transform is within the FOV of the light source...
        /// </summary>
        /// <param name="target">The transform that needs to be queryied to see if its inside the FOV</param>
        /// <returns>true/false based on if a transform is inside the FOV</returns>
        public Transform CheckFOV(Transform target, LayerMask ignoreLayers){
            Vector3 direction = (target.position - transform.position); // GETS DIRECTION BETWEEN BOTH VECTORS
            float angle = Vector3.Angle(direction, transform.up); // 2D orientated -- Get the angle difference between both the target and this object
            // THE RAYCAST DETECTION BE FUNKY!!! - TEST VISION CONE WITHOUT IT AGAIN!!
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, fovComponent.pointLightOuterRadius + 1f, ~ignoreLayers); // CAST A RAY IN THAT DIRECTION TO CHECK IF PLAYER IS WITHIN SIGHT...
            bool hasHitObj = hit && hit.collider && ((hit.collider.transform.GetInstanceID() == target.GetInstanceID()) || (hit.collider.transform.parent?.GetInstanceID() == target.GetInstanceID())); // If raycast hit the main Object or has hit "Character's" Parent object      
            fovComponent.color = new Color(fovComponent.color.r, fovComponent.color.g, fovComponent.color.b, (showDebug)? fovComponent.color.a : 0); // DEBUGGING
            if(!(angle < (visionAngle / 2) && hasHitObj)){ // IF OBJECT IS NOT IN THE VISION ARC OR IS NOT THE CORRECT OBJECT...    
                if(selectedTargets.Contains(target)){ selectedTargets.Remove(target); }                
                return null;
            }            
            if(showDebug){           
                if(!selectedTargets.Contains(target)){ selectedTargets.Add(target); }       
                
                float distance = (Mathf.Clamp((hit.transform)? (new Vector3(hit.point.x, hit.point.y) - transform.position).magnitude : direction.magnitude, 0, fovComponent.pointLightOuterRadius));
                Debug.DrawRay(transform.position, direction.normalized * distance, debugColor);
            }  
            return target;
        }

        /// <summary>
        /// This function will scan a surrounding area off this object and then parse all the found objects into the vision cone detection function to detect if its actually in-sight.
        /// </summary>
        /// <param name="targetLayers">These are the layers that will be queryied when checking for objects in-sight</param>
        /// <param name="selfObj">Used to make sure the calling object isnt detecting itself</param>
        /// <returns>A list of targets inside the vision cone to the calling script</returns>
        public List<Transform> GetTargets(LayerMask targetLayers, LayerMask ignoreLayers, Transform selfObj = null){
            List<Transform> targetsInArea = new List<Transform>();
            Collider2D[] objs = Physics2D.OverlapCircleAll(transform.position,  fovComponent.pointLightOuterRadius, targetLayers);            
            foreach(Collider2D obj in objs){                
                if(obj.transform == selfObj){ continue; } // Stops self detection...                
                Transform target = CheckFOV(obj.transform, ignoreLayers);
                if(target){ targetsInArea.Add(target); }
            }            
            return targetsInArea;
        }

        /// <summary>
        /// This function will scan a surrounding area off this object for any colliders based on the parsed layermask into the function
        /// </summary>
        /// <param name="targetLayers">These are the layers that will be queryied when checking for objects in-sight</param>
        /// <param name="checkRadius">Changes the distance this object will scan around to check for objects</param>
        /// <returns>An array of 2D colliders back to the calling script.</returns>
        public Collider2D[] GetObstacles(Transform selfObj, LayerMask targetLayers, float checkRadius){            
            debugCheckRadius = checkRadius;
            List<Collider2D> objs = Physics2D.OverlapCircleAll(selfObj.position,  checkRadius, targetLayers).ToList();

            // Remove self from array
            Collider2D colToRemove = null;
            objs.ForEach(col => {
                if(col.transform == selfObj){ colToRemove = col; }
            });
            objs.Remove(colToRemove);

            return objs.ToArray();
        }



        /// <summary>
        /// This function is simply used for debugging, and shows all targets inside a given area
        /// </summary>
        private void OnDrawGizmos() {
            if(!(showDebug && Application.isPlaying) || selectedTargets.Count <=0){ return; }

            Gizmos.color = debugColor;
            foreach(Transform selectedTarget in selectedTargets){
                if(!selectedTarget){ continue; }
                Gizmos.DrawWireCube(selectedTarget.position, selectedTarget.GetChild(0).GetComponent<Collider2D>().bounds.size);
            }            
            Gizmos.DrawWireSphere(transform.position, debugCheckRadius);
        }
    }
}