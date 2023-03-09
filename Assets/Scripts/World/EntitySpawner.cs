using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JoshGames.CharacterController;
using JoshGames.ItemStorage;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class EntitySpawner : MonoBehaviour{
    [Serializable] public class SpawnSettings{
        [HideInInspector] public float timeTillNextSpawn;

        public Collider2D region;
        public EntitySettings[] entities;
        public MinMax spawnIntevals;
        public GameObject characterHolder;

        public void UpdateEntityList(){
            foreach(EntitySettings entity in entities){
                for(int i = 0; i < entity.entities.Count; i++){
                    if(entity.entities[i]){ continue; }
                    entity.entities.RemoveAt(i);
                    i--;
                }
            }            
        }
    }
    [Serializable] public class EntitySettings{
        public GameObject entity;
        public EntityType entityType;
        public Transform spawnFolder;
        public int maxEntities;
        [HideInInspector] public List<GameObject> entities;

        // WHAT THIS ENTITY IS
        [HideInInspector] public CharacterBase character;        
        [HideInInspector] public Inventory_ItemStats item;
    }

    [Serializable] public class MinMax{
        public int min, max;
    }
    [SerializeField] SpawnSettings[] regions;

    private void Awake() {
        foreach(SpawnSettings region in regions){
            foreach(EntitySettings entity in region.entities){
                GameObject entityObj = entity.entity;
                entity.character = entityObj.GetComponent<CharacterBase>();
                entity.item = entityObj.GetComponent<Inventory_ItemStats>();
            }
        }
    }

    private void Update() {
        foreach(SpawnSettings region in regions){
            region.UpdateEntityList();

            if(Time.time >= region.timeTillNextSpawn){
                List<int> potentialSpawns = new List<int>();
                for(int i = 0; i < region.entities.Length; i++){
                    if(region.entities[i].entities.Count >= region.entities[i].maxEntities){ continue; }
                    potentialSpawns.Add(i);
                }
                if(potentialSpawns.Count <= 0){ continue; }
                int rndIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f) * (potentialSpawns.Count - 1));
                int randomSpawnIndex = potentialSpawns[rndIndex];
                EntitySettings chosenEntity = region.entities[randomSpawnIndex];

                Vector3 regionPos = region.region.bounds.center;
                Vector3 regionSize = region.region.bounds.extents;

                Vector3 rndSpawnPos = new Vector3(
                    UnityEngine.Random.Range(regionPos.x - regionSize.x, regionPos.x + regionSize.x),
                    UnityEngine.Random.Range(regionPos.y - regionSize.y, regionPos.y + regionSize.y)
                );


                GameObject entity = Instantiate(chosenEntity.entity, rndSpawnPos, Quaternion.identity, chosenEntity.spawnFolder);

                if(chosenEntity.entityType == EntityType.ITEM){
                    Inventory_ItemStats _item = entity.GetComponent<Inventory_ItemStats>();
                    _item.itemSettings.isLootable = true;
                    _item.quantityOfItem = UnityEngine.Random.Range(1, Mathf.FloorToInt((float)_item.GetItem().maxStack / 4));
                    _item.gameObject.layer = LayerMask.NameToLayer("Item");
                }
                else if(chosenEntity.entityType == EntityType.CHARACTER){
                    GameObject holder = Instantiate(region.characterHolder, rndSpawnPos, Quaternion.identity, chosenEntity.spawnFolder);
                    holder.GetComponentInChildren<UI_Health>().character = entity.GetComponent<CharacterBase>();
                    holder.GetComponentInChildren<UI_FollowTarget>().Target = entity.transform;
                    entity.transform.parent = holder.transform;                    
                }
                region.entities[randomSpawnIndex].entities.Add(entity);
                region.timeTillNextSpawn = Time.time + UnityEngine.Random.Range(region.spawnIntevals.min, region.spawnIntevals.max);
            }
        }
    }
    [Serializable] public enum EntityType{
        NONE,
        ITEM,
        CHARACTER,        
    }

}
