using UnityEngine;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
    This script only touches the avoid weights. Interest weights are handled in other scripts.
*/

namespace JoshGames.AI.Steering{
    public class SteeringAvoidance : SteeringBase{
        float[] avoidResult = null; // For gizmos

        /// <summary>
        /// This function dictates where an obstacle is, so that a processing script to handle which direction should be taken
        /// </summary>
        /// <param name="weights">Class holding arrays of 'weights' for each 8 directions surrounding a vector position. (Avoid & Interest)</param>        
        /// <returns>Class holding arrays of 'weights' for each 8 directions.</returns>
        public override SteeringWeights GetSteering(SteeringWeights weights, AIBrain aI, Vector2 targetPosition){
            if(!transform){
                Debug.LogError("transform == null || Transform needs to be assigned before calling");
                return null;
            }

            foreach(Collider2D obCollider in aI.obstacles){                
                if(obCollider.transform == transform){ continue; } // Stops the agent comparing itself
                
                Vector2 dirToObstacle = obCollider.ClosestPoint(transform.position) - (Vector2) transform.position; // Get the direction from the closest point of a collider to this character...
                Vector2 dirToObNormal = dirToObstacle.normalized; // Normalise the direction
                float distToOb = dirToObstacle.magnitude; // Get the numerical distance between both vectors

                float weight = (distToOb <= (Mathf.Max(aI.thisCollider.bounds.size.x, aI.thisCollider.bounds.size.y) +.1f))? 1 : (aI.sensorSettings.collisionCheckRadius - distToOb) / aI.sensorSettings.collisionCheckRadius; // Calculates how close it is to the collider. | Greater the weight, the more the agent will avoid it

                // Iterate through each direction, comparing the direction from character to the normalised direction towards the obstacle
                for(int i = 0; i < directionsAroundCharacter.Length; i++){
                    float result = Vector2.Dot(dirToObNormal, directionsAroundCharacter[i]); // Gets the dot product from the normalised direction towards the obstacle from the character to the normalised direction of each direction assigned to the character
                    float valueToAppend = result * weight; // Multiply the result by the severity e.g. How close the obstacle actually is to the character

                    if(valueToAppend > weights.avoid[i]){ // If the new value holds a greater result than cached result e.g. holds a stronger weight of interest...
                        weights.avoid[i] = valueToAppend; // Cache the new weight to the array
                    }
                    avoidResult = weights.avoid; // Give the gizmos the information needed to show all the weighted results 
                }
            }
            return weights;
        }

        /// <summary>
        /// This shows in 'red', Where the agent should avoid due to things like collisions etc...
        /// </summary>
        protected override void OnDrawGizmos(){
            base.OnDrawGizmos(); // Checks to see if gizmos are enabled on this steering behaviour
            if(avoidResult == null){ return; }

            Gizmos.color = Color.red;
            for(int i = 0; i < avoidResult.Length; i++){
                Gizmos.DrawRay(transform.position, directionsAroundCharacter[i] * avoidResult[i]); // Draws a line from transform to a direction multiplied by a weight
            }          
        }
    }
}