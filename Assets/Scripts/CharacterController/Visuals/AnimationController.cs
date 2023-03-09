using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.CharacterController{
    public class AnimationController : MonoBehaviour{
        [Serializable] struct BlinkTime{
            public float minTime, maxTime;
        }

        [SerializeField] Animator animator;
        [SerializeField] Velocity velocity;    
        [SerializeField] Transform parentObj;
        [SerializeField] BlinkTime blinkTimes;
        [SerializeField] bool hasNoseAnimation;
        [SerializeField] BlinkTime noseTimes;

        float timeTillNextBlink, timeTillNextSniffle;

        // UPDATES THE TIME TILL THE CHRACTER WILL BLINK AGAIN
        void UpdateBlinkTime() => timeTillNextBlink = Time.time + UnityEngine.Random.Range(blinkTimes.minTime, blinkTimes.maxTime);
        void UpdateNoseTime() => timeTillNextSniffle = Time.time + UnityEngine.Random.Range(noseTimes.minTime, noseTimes.maxTime);

        private void Awake(){
            animator = (animator) ? animator : GetComponent<Animator>();
            UpdateBlinkTime();
            UpdateNoseTime();
        }

        private void Update() {            
            float movingVelocity = Mathf.Ceil(velocity.value.normalized.magnitude);
            animator.SetBool("IsMoving", (velocity.value.normalized.magnitude >0)? true: false);  

            float nearestValue = Mathf.Round(velocity.value.normalized.x);

            bool isMovingVertical = nearestValue == 0 && velocity.value.normalized.y != 0;
            nearestValue = (isMovingVertical)? 1: nearestValue;   


            animator.SetFloat("Direction", (!isMovingVertical)? parentObj.localScale.x * nearestValue: nearestValue);

            if(Time.time >= timeTillNextBlink){
                UpdateBlinkTime();
                animator.SetTrigger("Blink");
            }

            if(hasNoseAnimation && Time.time >= timeTillNextSniffle){
                UpdateNoseTime();
                animator.SetTrigger("Sniffle");
            }
        }        
    }
}