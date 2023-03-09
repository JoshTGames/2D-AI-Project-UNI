using UnityEngine;
using System;
using TMPro;
using JoshGames.CharacterController;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

// THIS SCRIPT IS DESIGNED WITH INHERITANCE IN MIND SO THAT IT HELPS DESIGNING NEW ITEMS AND ALSO FOR APPENDING AND FIGURING OUT WHAT ITEM AN ITEM IS
namespace JoshGames.ItemStorage{    
    public class Item : ScriptableObject{
        public bool showGizmos;
        [Serializable] public struct WeaponData{
            [Tooltip("This is the damage the object or the projectile will inflict")] public float weaponDamage;
            [Tooltip("Time till the next attack can be made")] public float fireRate;
            [Tooltip("Time it takes for attack to be performed")] public float timeToAttack;
            [Tooltip("Time it takes to change the magazine")] public float reloadTime;

            public int magCapacity;
            public float sightRange;
            [Tooltip("If assigned, this will be instantiated when triggered")] public Ammunition projectile;
            public bool hasAoeAttack;

            [Header("Idle Holding data (ON THE HANDS)")]
            public float positionMultiplierFromCharacterIdle;            
            public float rotateAmountIdle;
            public float idleSmoothing;
            
            [Header("Animation data (ON THE HANDS)")] 
            public float positionMultiplierFromCharacterActivate; 
            public float rotateAmountActivate;
            public float activateSmoothing;

            [Header("Audio settings")]            
            public AudioClip[] onTriggerAudio;
            public AudioClip[] onImpactAudioForMelee;
            public bool createDetectableSound;
        }

        [Serializable] public struct ConsumableData{
            public float buffAmount, duration, cooldown;
        } 

        [Serializable] public struct AmmunitionData{
            public float speed;
            public float lifetime;
            public Sprite itemBox;
            [Tooltip("This will dictate how many colliders this object can pass through before breaking.")] public int penetration;

            public GameObject onImpactFX;
            [Header("Audio settings")]            
            public AudioClip[] onImpactAudio;
            public GameObject audioSource;
        }

        [Serializable] public struct ResourceData{}

        [Serializable] public enum ItemType{        
            Weapon,
            Consumable,
            Resource,
            Ammunition
        }
    
        /// <summary>
        /// This function is in charge of activating an items potential ability (If it has one)
        /// </summary>
        public virtual void Activate(Transform _owner, Inventory_ItemStats thisObj, HandToMouse handController, RotateToMouse handRotationController){}

        /// <summary>
        /// This function will be in charge of positioning the hands to fit the item
        /// </summary>
        public virtual void Equip(CharacterBase _owner,Inventory_ItemStats thisObj, HandToMouse handController, RotateToMouse handRotationController, int handIndex = 0){}


        // MAIN VARIABLES
        // --------------

        [Tooltip("Max you can hold of this item")] public int maxStack = 1;
        [Tooltip("This is the physical item itself")] public GameObject itemPrefab;
        [Tooltip("Simply an image resembling the item. (This will be whats seen by the player inside the inventory)")] public Sprite itemImage;
        [TextArea] public string itemDescription;
        [Tooltip("Possibly the rarity of the item?")] public VertexGradient itemColour;    
        [Tooltip("Noise that is played when equipped")] public AudioClip equipNoise;    
        
        [Tooltip("Open this if you are creating a weapon")] [HideInInspector] public WeaponData weaponData{ get; protected set; }
        [Tooltip("Open this if you are creating a consumable")] [HideInInspector] public ConsumableData consumableData{ get; protected set; }
        [Tooltip("Open this if you are creating a consumable")] [HideInInspector] public AmmunitionData ammunitionData{ get; protected set; }
        [Tooltip("Open this if you are creating a consumable")] [HideInInspector] public ResourceData resourceData{ get; protected set; }

        [Tooltip("Helps the system figure out how to handle this object")] public ItemType itemType{ get; protected set; }
    }
}