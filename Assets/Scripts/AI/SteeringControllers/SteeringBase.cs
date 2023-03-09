using UnityEngine;
using System;
using System.Collections.Generic;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---

    How this steering algoritm works, is simply that the solver will pick a direction with the strongest weight.
    By subtracting the avoid array from the interest array, you can find a direction from the 8 directions which has the greater value.
*/

namespace JoshGames.AI.Steering{
    public abstract class SteeringBase{
        // This holds normalized directions which would in theory be used as a multiplier around a character. Theses directions would then be multiplied by a value e.g. Weight.
        public static Vector2[] directionsAroundCharacter = new Vector2[]{
            new Vector2(1, 1).normalized,
            new Vector2(1, 0).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(0, -1).normalized,
            new Vector2(-1, -1).normalized,        
            new Vector2(-1, 0).normalized,        
            new Vector2(-1, 1).normalized,        
            new Vector2(0, 1).normalized,        
        };

        public bool showGizmos = false; // When enabled, will show the gizmos associated to the steering behaviour
        public Transform transform; // Because this isn't inheriting from monobehaviour... if we want the transform we will have to manually assign it

        /// <summary>
        /// This class holds 2 arrays, avoid & interest. Avoid is typically for things like collisions and interest is potential places to move towards.
        /// </summary>
        public class SteeringWeights{
            public float[] avoid, interest;

            public SteeringWeights(float[] _avoid, float[] _interest){
                avoid = _avoid;
                interest = _interest;
            }
        }

        /// <summary>
        /// This function acts as a base for steering behaviours to branch off of.
        /// </summary>
        /// <param name="weights">Class holding arrays of 'weights' for each 8 directions surrounding a vector position. (Avoid & Interest)</param>        
        /// <param name="aI">This is the processing module which will handle all the AI tasks such as</param>        
        /// <returns>Class holding arrays of 'weights' for each 8 directions.</returns>
        public abstract SteeringWeights GetSteering(SteeringWeights weights, AIBrain aI, Vector2 targetPosition);        

        /// <summary>
        /// This shows all gimos associated to an inheriting steering behaviour.
        /// </summary>
        protected virtual void OnDrawGizmos() {
            if(!(showGizmos && Application.isPlaying)){ return; }
        }
    }
}