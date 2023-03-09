using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
    This script only touches the interest weights. avoid weights are handled in other scripts.
*/

namespace JoshGames.AI.Steering{
    public class SteeringSeek : SteeringBase{
        bool reachedLastTarget = true; // If the agent is within distance... Then dont move any closer
        
        // GIZMO vars
        Vector2 lastTargetPos; // The last seen position the target was at
        float[] interestResult; // Array holding all the possible weighted directions
        //


        /// <summary>
        /// This function dictates where the target is and returns all possible weighted directions the agent could take
        /// </summary>
        /// <param name="weights">Class holding arrays of 'weights' for each 8 directions surrounding a vector position. (Avoid & Interest)</param>        
        /// <returns>Class holding arrays of 'weights' for each 8 directions.</returns>
        public override SteeringWeights GetSteering(SteeringWeights weights, AIBrain aI, Vector2 targetPosition){ 
            // MAIN SEEKING CODE
            Vector2 dirToTarget = (targetPosition - (Vector2)transform.position); // Get direction from between target and agent
            dirToTarget = (aI.isSeeking)? dirToTarget : -dirToTarget; // Means the agent is capable of safely fleeing without hitting obstacles

            for(int i = 0; i < weights.interest.Length; i++){
                float result = Vector2.Dot(dirToTarget.normalized, directionsAroundCharacter[i]); // Gets the dot product from the normalised direction towards the target from the character to the normalised direction of each direction assigned to the character

                if(result > 0){
                    float valueToAppend = result;
                    if(valueToAppend > weights.interest[i]){ // If the new value holds a greater result than cached result e.g. holds a stronger weight of interest...
                        weights.interest[i] = valueToAppend; // Cache the new weight to the array
                    }
                }
            }
            interestResult = weights.interest; // Give the gizmos the information needed to show all the weighted results 
            return weights;
        }


        /// <summary>
        /// This shows in 'green', Where the agent could go. It also shows the last seen location of the target in real-time
        /// </summary>
        protected override void OnDrawGizmos(){
            base.OnDrawGizmos(); // Checks to see if gizmos are enabled on this steering behaviour
            if(interestResult == null){ return; }

            Gizmos.color = Color.green;
            for(int i = 0; i < interestResult.Length; i++){
                Gizmos.DrawRay(transform.position, directionsAroundCharacter[i] * interestResult[i]);
            }
            if(!reachedLastTarget){
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(lastTargetPos, 0.1f); // THIS WILL REPRESENT THE LAST SEEN POSITION OF THE TARGET IN REALTIME
            }       
        }
    }
}