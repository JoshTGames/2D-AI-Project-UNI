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
    public class Inventory_AmmunitionStats : Inventory_ItemStats{
        public Ammunition thisItem;        
        [HideInInspector] public CharacterBase owner;   
        [HideInInspector] public float damage;
        [HideInInspector] public int collidersPenetrated;        
        Rigidbody2D rb;
        Collider2D thisCollider;
        Collider2D previousHitCollider;

        SpriteRenderer itemImage;

        [SerializeField] bool activate; // If true, values will be assigned to this object e.g. Initiate an attack
        public bool Activate{
            get{ return activate; }
            set{ 
                if(value){
                    rb = GetComponent<Rigidbody2D>();
                    // owner.Attack = false;
                    Destroy(gameObject, t: (thisItem)? thisItem.ammunitionData.lifetime : 0);
                }
                activate = value; 
            }
        }       


        private void OnEnable() => item = thisItem;    

        private void Awake() {
            itemImage = GetComponent<SpriteRenderer>();
        }
        
        private void FixedUpdate() {            
            // Move forward...
            if(activate && thisItem){
                rb.MovePosition(transform.position + (transform.right * (thisItem.ammunitionData.speed * Time.fixedDeltaTime)));
            }

            if(itemSettings.isLootable && itemImage.sprite != thisItem.ammunitionData.itemBox){
                itemImage.sprite = thisItem.ammunitionData.itemBox;
            }
        }

        /// <summary>
        /// This function allows for inheritance to take place and therefore easily create unique behaviours when a character has been hit
        /// </summary>
        /// <param name="character">The character to affect</param>
        protected virtual void Attack(CharacterBase character) => character.Damage(damage, (owner)? owner.transform.position : null);


        private void OnTriggerEnter2D(Collider2D other) { 
            if(!activate){ return; }          
            // Do damage if of characterbase type            
            CharacterBase character = other.transform.GetComponent<CharacterBase>();
            character = (!character)? other.transform.parent.GetComponent<CharacterBase>() : character;
            if(character == owner){ return; } // Ensures its not hitting owner...

            if(previousHitCollider && (other == previousHitCollider || other.transform == previousHitCollider?.transform.parent || (previousHitCollider?.transform.childCount > 0 && other.transform == previousHitCollider?.transform.GetChild(0)))){ // Stops the bullet attacking the same target multiple times
                return;
            }            
            collidersPenetrated++;
                        
            if(character){ Attack(character); }

            // Finally, destroy this object as it has collided with objects...
            previousHitCollider = other;
            Instantiate(thisItem.ammunitionData.onImpactFX, transform.position, Quaternion.identity, GameObject.Find("REPLICATED_STORAGE").transform);

            // Play sound FX
            AudioSource audioSource = Instantiate(thisItem.ammunitionData.audioSource, transform.position, Quaternion.identity, GameObject.Find("REPLICATED_STORAGE").transform).GetComponent<AudioSource>();
            audioSource.clip = thisItem.ammunitionData.onImpactAudio[Random.Range(0, thisItem.ammunitionData.onImpactAudio.Length)];
            audioSource.Play();

            // Create sound emission            
            SoundEmitter.instance.worldSounds.Add(new SoundEmitter.Sound(audioSource, owner, transform.position, Color.yellow));
            Destroy(audioSource.gameObject, audioSource.clip.length);
            if(collidersPenetrated >= thisItem.ammunitionData.penetration){ 
                if(transform.childCount >0){ transform.GetChild(0).parent = transform.parent; }
                Destroy(gameObject); 
            }
        }
        private void OnDrawGizmos() {            
            if(thisItem && thisItem.showGizmos){                
                Gizmos.color = (thisItem.itemColour.bottomRight + thisItem.itemColour.bottomRight + thisItem.itemColour.topLeft + thisItem.itemColour.topRight) / 4;                
                Gizmos.DrawWireSphere(transform.position, thisItem.weaponData.sightRange);
            }    
        }
    }
}