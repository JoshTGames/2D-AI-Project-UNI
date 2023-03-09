using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.CharacterController;
using JoshGames.AI.Sensors;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    public class Inventory_WeaponStats : Inventory_ItemStats{
        public Weapon thisItem;        
        [SerializeField] AudioSource audioSource;
        [SerializeField] MeleeAttack[] colliderHandlers;
        [SerializeField] ParticleSystem[] onTriggerParticles;

        [HideInInspector] public CharacterBase owner;        
        float delay; // E.g. Fire rate        
        public int magazine;
        bool attack;
        [HideInInspector] public bool isReloading;
        [HideInInspector] public HandToMouse handController;
        [HideInInspector] public RotateToMouse handRotationController;

        int curHandIndex, prvHandIndex;
        int CurHandIndex{
            get{ return curHandIndex; }
            set{              
                if(colliderHandlers.Length >0){ colliderHandlers[value].activateThis = true; }  

                prvHandIndex = curHandIndex;
                curHandIndex = value;
            }
        }

        #region InputData
        // These values are modified in realtime by a scriptable object        
        public float Delay{ // Just a way to reduce the word count... Will always increment the value needed onto time.
            get{ return delay; }
            set{
                delay = Time.time + value;
            }
        }
        public bool Attack{
            get{ return attack; }
            set{ 
                bool canAttack = (Time.time >= delay && ((magazine >0 && item.weaponData.projectile) || !item.weaponData.projectile))? value : false; // ALSO CHECK IF THERE IS AMMO TO TAKE FROM THE INVENTORY
                if(canAttack == attack){ return; } // Stops repetitive calls
                attack = canAttack;
                if(attack){        
                    Delay = thisItem.weaponData.timeToAttack;              
                    CurHandIndex = (thisItem.name == "Fists") ? (CurHandIndex + 1) % handController.hands.Length : 0;     

                    foreach(ParticleSystem pS in onTriggerParticles){ pS.Play(); } // Play particle FX

                    // Play SFX
                    audioSource.clip = thisItem.weaponData.onTriggerAudio[Random.Range(0, thisItem.weaponData.onTriggerAudio.Length-1)];          
                    audioSource.Play();
                    // Create sound emission
                    if(thisItem.weaponData.projectile){
                        SoundEmitter.instance.worldSounds.Add(new SoundEmitter.Sound(audioSource, owner, transform.position, Color.red));
                    }
                }
                else if(colliderHandlers.Length >0){
                    colliderHandlers[prvHandIndex].activateThis = false;
                }

                if(!attack){
                    Delay = thisItem.weaponData.fireRate;
                }
            } // Will only be allowed to set to true after the delay has been negated.
        }
       
        #endregion

        private void OnEnable() => item = thisItem;

        private void Awake() {
            colliderHandlers = (colliderHandlers == null || colliderHandlers.Length <= 0)? colliderHandlers : GetComponentsInChildren<MeleeAttack>();
            Delay = thisItem.weaponData.reloadTime;
        }

        

        private void Update() {
            if(!handController){ return; }

            if(thisItem.weaponData.projectile && magazine <= 0 && Time.time >= Delay){ 
                // Scan inventory for the required ammunition
                Dictionary<int, List<Item>> invData = owner.inventory.GetData(owner.gameObject.tag);
                List<int> ammoIndexes = owner.inventory.GetItemIndexesOfType(Item.ItemType.Ammunition, owner.gameObject.tag);
                foreach(int index in ammoIndexes){
                    if(invData[index][0].name == item.weaponData.projectile?.name){
                        magazine += owner.inventory.Remove(owner.gameObject.tag, index, item.weaponData.magCapacity - magazine);
                    }
                }      
                if(magazine >0){
                    Delay = item.weaponData.reloadTime; 
                    isReloading = true;
                }          
            }

            if(Time.time >= Delay){ isReloading = false; }

            
            if(Time.time >= Delay && Attack){ Attack = false; }            
            handController.hands[CurHandIndex].targetPositionMultiplier = (Attack)? thisItem.weaponData.positionMultiplierFromCharacterActivate : thisItem.weaponData.positionMultiplierFromCharacterIdle;
            handController.hands[CurHandIndex].smoothingOverride = (Attack)? thisItem.weaponData.activateSmoothing : thisItem.weaponData.idleSmoothing;
            
            handRotationController.objectsToRotate[CurHandIndex].overrideAngle = (Attack)? thisItem.weaponData.rotateAmountActivate : thisItem.weaponData.rotateAmountIdle;     
            handRotationController.objectsToRotate[CurHandIndex].smoothingOverride = (Attack)? thisItem.weaponData.activateSmoothing : thisItem.weaponData.idleSmoothing;
        }

        private void OnDrawGizmos() {            
            if(thisItem && thisItem.showGizmos){                
                Gizmos.color = (thisItem.itemColour.bottomRight + thisItem.itemColour.bottomRight + thisItem.itemColour.topLeft + thisItem.itemColour.topRight) / 4;                
                Gizmos.DrawWireSphere(transform.position, thisItem.weaponData.sightRange);
            }    
        }
    }
}