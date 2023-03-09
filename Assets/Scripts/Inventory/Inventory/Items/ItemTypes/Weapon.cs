using JoshGames.CharacterController;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.ItemStorage{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Inventory/Create Weapon")]
    public class Weapon : Item{       
        [SerializeField] WeaponData itemSettings;       
        bool isRangedWeapon;        
        [HideInInspector] public Transform ammoStorage;
        

        private void OnEnable() {
            itemType = ItemType.Weapon;
            weaponData = itemSettings;

            isRangedWeapon = (itemSettings.projectile != null);            
        }

        public override void Activate(Transform _owner, Inventory_ItemStats thisObj, HandToMouse handController, RotateToMouse handRotationController){            
            base.Activate(_owner, thisObj, handController, handRotationController);            
            Inventory_WeaponStats weapon = (Inventory_WeaponStats) thisObj;
            
            // Play animation...
            RunItem(); // Run this if no animation is parsed           
            
            
            if(!weapon.Attack){
                weapon.Attack = true;
                if(isRangedWeapon && weapon.Attack){
                    weapon.magazine--;
                    GameObject ammoObj = Instantiate(itemSettings.projectile.itemPrefab, weapon.transform.GetChild(1).position, Quaternion.FromToRotation(itemSettings.projectile.itemPrefab.transform.right, weapon.transform.right), ammoStorage);
                    Inventory_AmmunitionStats ammo = ammoObj.GetComponent<Inventory_AmmunitionStats>();
                    ammo.Activate = true;
                    ammo.damage = itemSettings.weaponDamage;
                    ammo.owner = weapon.owner;
                }                
            }
        }

        public override void Equip(CharacterBase _owner, Inventory_ItemStats thisObj, HandToMouse handController, RotateToMouse handRotationController, int handIndex = 0){
            base.Equip(_owner, thisObj, handController, handRotationController, handIndex);  
            ammoStorage = GameObject.Find("REPLICATED_STORAGE")?.transform;
            if(thisObj.transform.childCount >0){
                thisObj.transform.localPosition = -thisObj.transform.GetChild(handIndex).localPosition;
            }          

            Inventory_WeaponStats weapon = (Inventory_WeaponStats) thisObj;
            weapon.owner = _owner;

            weapon.handController = (weapon.handController)? weapon.handController : handController;
            weapon.handRotationController = (weapon.handRotationController)? weapon.handRotationController : handRotationController;

            for(int i = 0; i < handController.hands.Length; i++){
                handController.hands[i].targetPositionMultiplier = weaponData.positionMultiplierFromCharacterIdle;
                handController.hands[i].smoothingOverride = weaponData.idleSmoothing;
            }
            
            AudioSource source = weapon.GetComponent<AudioSource>();
            source.clip = equipNoise;
            source.Play();         
        }

        /// <summary>
        /// For more unique weapons which needs the extra functionality...
        /// </summary>
        public virtual void RunItem(){
        }
    }
}