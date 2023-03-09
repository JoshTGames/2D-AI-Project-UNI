using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.CharacterController{
    public class Velocity : MonoBehaviour{
        [HideInInspector] public Vector3 value; // NORMALISED VALUE BETWEEN -1 - 1 REPRESENTING THE DIRECTION THE CHARACTER IS MOVING AT
        Vector3 previousPosition;

        private void Start() => previousPosition = transform.position;
        private void FixedUpdate(){
            value = (transform.position - previousPosition) / Time.fixedDeltaTime;
            previousPosition = transform.position;
        }    
    }
}