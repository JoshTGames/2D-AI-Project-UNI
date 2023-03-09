using UnityEngine;
using System;
using System.Collections.Generic;
using JoshGames.Interaction;
using JoshGames.ItemStorage;
using JoshGames.AI.Sensors;
using UnityEngine.Tilemaps;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.CharacterController{
    public abstract class CharacterBase : MonoBehaviour{
        protected Rigidbody2D physicsAgent;
        protected Velocity velocity;
        
        Vector3 positionSmoothingPosition, smoothingVelocity;

        public Inventory inventory;
        public Item EquippedItem{
            get{ return inventory.equippedItem; }
            set{
                if(!inventory){ return; }
                inventory.equippedItem = ChangeHeldObject(value);
            }
        }

        protected Inventory_ItemStats equippedItemVisual;        
            
        [SerializeField] Inventory_ItemStats hands;
        [SerializeField] HandToMouse handController;
        [SerializeField] RotateToMouse handRotationController;
        public Transform itemHolder;

        public IInteractable Interactable { get; set; }       
        [SerializeField] AudioSource audioManagerFeet;
        [SerializeField] AudioClip[] footstepNoises;
        [Serializable] protected class Stats{
            public float maxHealth = 1;
            public float health = 1;
            public float speed = 1;
            [Tooltip("This will replace the equipped item slot to stop the value being null.")] public Item[] defaultHeldItems = null; // When nothing is equipped this will be active on the player...
            public ParticleSystem blood;
            public ParticleSystem deathFX;
        }
        protected Stats profile = new Stats();
        public Tilemap bounds;
        [HideInInspector] public bool updateHeldSlot;
        [SerializeField] Collider2D impactCollider;

        protected virtual void Start() {
            physicsAgent = GetComponent<Rigidbody2D>();
            velocity = GetComponent<Velocity>();            

            if(!inventory){ return; }
            EquippedItem = (EquippedItem == null && profile.defaultHeldItems.Length >0 && profile.defaultHeldItems[0])? profile.defaultHeldItems[0] : EquippedItem;
            // Add this item to the inventory
            for(int i = 0; i < profile.defaultHeldItems.Length; i++){ inventory.Add(gameObject.tag, i, profile.defaultHeldItems[i]); }
        }
        
        protected virtual void Move(Vector3 direction, float speed, float smoothing = 0){            
            positionSmoothingPosition = Vector3.SmoothDamp(positionSmoothingPosition, direction * speed, ref smoothingVelocity, smoothing);

            Vector3 boundsSize = Vector3.Scale(((Vector3)bounds.size / 2), bounds.cellSize);
            Vector3 clampedPos = ClampedPosition.GetClampedPos(transform.position + positionSmoothingPosition * Time.fixedDeltaTime, bounds.transform.position, boundsSize);
            physicsAgent.MovePosition(clampedPos);            
        }

        protected virtual void Move(Vector3 position){ // FOR NAVMESHING
        }


        public virtual void Damage(float damage, Vector3? hittersPosition = null){ // FOR DAMAGING THE CHARACTER
            // Perform effects....
            ParticleSystem bloodFX = Instantiate(profile.blood, (hittersPosition != null)? impactCollider.ClosestPoint((Vector3)hittersPosition) : transform.position, Quaternion.identity, transform);
            // Adds hitters position to memory or something...
        }
        
        /// <summary>
        /// This function returns the health + maxhealth of a character
        /// </summary>
        /// <param name="_health">The concurrent health of the character</param>
        /// <param name="_maxHealth">The most possible health this character can have</param>
        public abstract void GetHealth(out float _health, out float _maxHealth);
        
        public virtual void Death(){            
            Instantiate(profile.deathFX, transform.position, Quaternion.identity, GameObject.Find("REPLICATED_STORAGE").transform);
            Destroy(transform.parent.gameObject);
        }

        public bool PickUpItem(Inventory_ItemStats _item){
            if(!_item.CanPickup(transform)){ return false; }

            Dictionary<int, List<Item>> invData = inventory.GetData(tag);
            bool isWeapon = (_item.GetItem().itemType == Item.ItemType.Weapon);
            if(isWeapon){
                for(int i = 0; i < inventory.Slots; i++){
                    _item.quantityOfItem = inventory.Add(tag, i, _item.GetItem(), _item.quantityOfItem);
                    if(_item.quantityOfItem <= 0){ 
                        Destroy(_item.gameObject); 
                        break;
                    }                
                }
                updateHeldSlot = true;
            }
            else{
                for(int i = inventory.Slots; i > 0; i--){
                    _item.quantityOfItem = inventory.Add(tag, i, _item.GetItem(), _item.quantityOfItem);
                    if(_item.quantityOfItem <= 0){ 
                        Destroy(_item.gameObject); 
                        break;
                    }                
                }
            }            
            return true;
        }

        public bool DropItem(GameObject prefab, int amount = 1, int? slotIndex = null){
            GameObject storage = GameObject.Find("REPLICATED_STORAGE");
            Collider2D col = GetComponent<Collider2D>();

            if(slotIndex != null){ inventory.Remove(this.tag, (int)slotIndex, amount); }
            
            for(int i = amount; i > 0; i--){
                Vector3 rndSpawnPos = new Vector3(
                    UnityEngine.Random.Range(col.bounds.center.x - col.bounds.extents.x, col.bounds.center.x + col.bounds.extents.x),
                    UnityEngine.Random.Range(col.bounds.center.y - col.bounds.extents.y, col.bounds.center.y + col.bounds.extents.y)
                );
                Quaternion direction = Quaternion.FromToRotation(prefab.transform.right, handController.GetPrimaryHand().right);
                Inventory_ItemStats droppedItem = Instantiate(prefab, rndSpawnPos, direction, storage.transform).GetComponent<Inventory_ItemStats>();
                droppedItem.gameObject.layer = LayerMask.NameToLayer("Item");
                droppedItem.quantityOfItem = Mathf.Min(amount, droppedItem.GetItem().maxStack);
                droppedItem.itemSettings.isLootable = true;
                amount -= Mathf.Min(amount, droppedItem.GetItem().maxStack);
            }
            updateHeldSlot = true;
            return false;
        }

        /// <summary>
        /// This will activate the behaviour inside the item it is holding.
        /// </summary>
        public virtual void UseHeldObject(Inventory_ItemStats _itemVisual) => EquippedItem.Activate(this.transform, _itemVisual, handController, handRotationController);
        
        /// <summary>
        /// This function will change the object this character is holding in its hands
        /// </summary>
        /// <param name="newItem">This is the replacing item that this character will hold.</param>
        /// <returns>The item to be held</returns>
        private Item ChangeHeldObject(Item newItem = null){  
            newItem = (newItem != null || profile.defaultHeldItems.Length <= 0)? newItem : profile.defaultHeldItems[0]; // Ensures newItem holds a value... 
            if(newItem == null){ newItem = hands.GetItem(); }

            if(equippedItemVisual && equippedItemVisual.GetType() == typeof(Inventory_WeaponStats)){
                Inventory_WeaponStats selectedWep = (Inventory_WeaponStats) equippedItemVisual;
                if(selectedWep && selectedWep.magazine >0){
                    int remainingMag = selectedWep.magazine; 
                    Dictionary<int, List<Item>> invData = inventory.GetData(gameObject.tag);
                    List<int> ammoIndex = inventory.GetItemIndexesOfType(Item.ItemType.Ammunition, gameObject.tag);     
                    foreach(int index in ammoIndex){
                        remainingMag = inventory.Add(gameObject.tag, index, equippedItemVisual.GetItem().weaponData.projectile, remainingMag);
                        if(remainingMag <= 0){ break; }
                    }
                    // If a new slot is required...
                    if(remainingMag >0){
                        for(int i = inventory.Slots; i > 0; i--){
                            if(invData.ContainsKey(i)){ continue; }
                            remainingMag = inventory.Add(gameObject.tag, i, equippedItemVisual.GetItem().weaponData.projectile, remainingMag);
                            if(remainingMag <= 0){ break; }
                        }
                        // If remainingMag is STILL greater than 0... Drop it on the floor!
                        if(remainingMag >0){
                            DropItem(equippedItemVisual.GetItem().weaponData.projectile.itemPrefab, remainingMag);
                        }
                    }
                }
            }

            // replace equipped item visuals and instantiate new ones (If exists)...
            if(EquippedItem?.itemPrefab && (equippedItemVisual.transform.GetInstanceID() != hands.transform.GetInstanceID())){ Destroy(equippedItemVisual?.gameObject); }

            if(newItem?.itemPrefab){                
                equippedItemVisual = Instantiate(newItem.itemPrefab, itemHolder.position, Quaternion.FromToRotation(newItem.itemPrefab.transform.right, itemHolder.right), itemHolder).GetComponent<Inventory_ItemStats>();
                equippedItemVisual.gameObject.layer = handController.GetPrimaryHand().gameObject.layer;
            }
            else{ equippedItemVisual = hands; }
            newItem.Equip(this, equippedItemVisual, handController, handRotationController);
            return newItem;
        }

        public Inventory_ItemStats GetEquippedItemVisual() => equippedItemVisual;

        public void PlayFootstepSound(){
            AudioClip selectedNoise = footstepNoises[UnityEngine.Random.Range(0, footstepNoises.Length)];
            audioManagerFeet.clip = selectedNoise;
            audioManagerFeet.Play();
            SoundEmitter.instance.worldSounds.Add(new SoundEmitter.Sound(audioManagerFeet, this, transform.position, Color.blue));            
        }

        protected virtual void Update() {            
            if(profile != null && profile.health <= 0){ Death(); }
        }
    }
}