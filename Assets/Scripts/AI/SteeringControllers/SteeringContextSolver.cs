using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

namespace JoshGames.AI.Steering{
    public class SteeringContextSolver : MonoBehaviour{
        [SerializeField] bool showGizmos; // If enabled, this will show the direction the agent will be moving towards.

        // GIZMO vars        
        Vector2 resultDir; // The direction to draw the ray the agent will be moving towards.
        float rayLength = 1; // The max length in the inspector this will draw the direction this agent will move towards. 

        SteeringBase.SteeringWeights gizmoWeights; // This will show all the weights around the character....
        //
        
        /// <summary>
        /// This function calculates the direction the agent WILL move to from given steering behaviours parsed into it.
        /// </summary>
        /// <param name="behaviours">List of steering behaviours this agent will use to dictate where it will move to</param>        
        /// <param name="aI">The agent itself</param>        
        /// <returns>The direction this agent should actually move towards</returns>
        public Vector2 GetDirToMove(List<SteeringBase> behaviours, AIBrain aI, Vector2 targetPosition){
            int directionSlots = SteeringBase.directionsAroundCharacter.Length; // Gets the number of directions.

            SteeringBase.SteeringWeights weights = new SteeringBase.SteeringWeights( // Creates new arrays with the x amount of directions stated above.
                new float[directionSlots],
                new float[directionSlots]
            );            

            foreach(SteeringBase behaviour in behaviours){ 
                behaviour.showGizmos = showGizmos; // Enables gizmos in this steering behaviour
                weights = behaviour.GetSteering(weights, aI, targetPosition); // Gets the steering data from each behaviour
            } 
            gizmoWeights = new SteeringBase.SteeringWeights(weights.avoid, weights.interest);

            Vector2 outputDir = Vector2.zero; // This will be all the directions combined.
            for(int i = 0; i < directionSlots; i++){ // Iterates each direction given.
                weights.interest[i] = Mathf.Clamp01(weights.interest[i] - weights.avoid[i]); // Subtracts the danger value from the interest value (This will heavily dictate the direction value)
                outputDir += SteeringBase.directionsAroundCharacter[i] * weights.interest[i]; // Direction * weighting...
            }

            resultDir = outputDir.normalized; // Averages out the total direction to get the direction that the agent will move towards
            return resultDir; 
        }
        
        /// <summary>
        /// This shows in 'yellow', Where the agent is currently moving to.
        /// </summary>
        private void OnDrawGizmos() {
            if(!(showGizmos && Application.isPlaying && gizmoWeights != null)){ return; }
            Gizmos.color = Color.yellow;            
            Gizmos.DrawRay(transform.position, resultDir * rayLength);

            Gizmos.color = Color.red;
            for(int i = 0; i < gizmoWeights.avoid.Length; i++){
                Gizmos.DrawRay(transform.position, SteeringBase.directionsAroundCharacter[i] * gizmoWeights.avoid[i]); // Draws a line from transform to a direction multiplied by a weight
            }    

            Gizmos.color = Color.green;
            for(int i = 0; i < gizmoWeights.interest.Length; i++){
                Gizmos.DrawRay(transform.position, SteeringBase.directionsAroundCharacter[i] * gizmoWeights.interest[i]); // Draws a line from transform to a direction multiplied by a weight
            }     
        }
    }
}