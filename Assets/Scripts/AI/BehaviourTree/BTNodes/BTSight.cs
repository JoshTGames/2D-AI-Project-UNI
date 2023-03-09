using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.CharacterController;
using JoshGames.ItemStorage;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTSight : BTNode{
    AIBrain ai;
    AIMemory memory;
    Inventory inventory;
    public BTSight(AIBrain _ai, AIMemory _memory, Inventory _inventory){ 
        this.ai = _ai;
        this.memory = _memory;
        this.inventory = _inventory;
    }


    /// <summary>
    /// This function will generate a task to be performed later by the AI
    /// </summary>
    /// <param name="transform">This is the object the task will be generated around</param>
    /// <param name="behaviour">This will dictate what task is generated</param>
    /// <returns>The task to be soon added to memory</returns>
    BTNode GenerateTask(Transform transform, AISettings.Behaviour behaviour){
        BTNode task = null;
        
        BTScanInventoryForItemType SCAN_FOR_WEAPONS = new BTScanInventoryForItemType(ai, inventory, Item.ItemType.Weapon); // Scans the inventory for weapons
        BTSelectWeapon SELECT_WEAPON = new BTSelectWeapon(ai, inventory, transform); // Selects a weapon best suited to the distance between the target and agent
        BTSequence GET_WEAPON = new BTSequence(new List<BTNode>{SCAN_FOR_WEAPONS, SELECT_WEAPON}); // Sequences the above 2 actions together
        
        BTMove FLEE = new BTMove(ai, transform.position + (transform.position - ai.transform.position).normalized, ai.agentFromTargetDelta, false); // Flee from target        
        BTLookAt FACETARGET = new BTLookAt(ai, transform.position); // Faces the target that approached it
        BTWait WAIT = new BTWait(ai, .5f); // Wait doesn't need to be a high value, just enough so the AI doesn't instantly freak out after seeing the character again
        BTSequence FLEE_FROM_POSITION = new BTSequence(new List<BTNode>{FLEE, FACETARGET, WAIT}); // Runs away from a position and then faces target to see if they still are there
                
        // FLEE, Facing target IF holding ranged weapon (Has projectile)
        BTIsHoldingRangedWeapon IS_HOLDING_RANGED_WEAPON = new BTIsHoldingRangedWeapon(ai);
        BTIsInRange ENEMY_IN_RANGE = new BTIsInRange(ai, transform, ai.vision.sightRange);
        BTActiveMove FLEE_FACETARGET = new BTActiveMove(ai, memory, transform, ai.vision.sightRange /2, false, true, false); // Flee but face target 
        BTSequence FLEE_RANGEDAI = new BTSequence(new List<BTNode>{IS_HOLDING_RANGED_WEAPON, ENEMY_IN_RANGE, FLEE_FACETARGET});    
        BTInverter INVERT_FLEE_RANGEDAI = new BTInverter(FLEE_RANGEDAI);
        
        BTAttack ATTACK = new BTAttack(ai, inventory, transform);
        BTSequence ATTACK_TARGET = new BTSequence(new List<BTNode>{GET_WEAPON, ATTACK, INVERT_FLEE_RANGEDAI});
        BTActiveMove CHASE = new BTActiveMove(ai, memory, transform, (inventory)? ai.EquippedItem.weaponData.sightRange : 0, true, false, (ai.agentProfile.chanceToInvestigate > 0)); // Chases live target
        
        switch(behaviour){
            case AISettings.Behaviour.Passive:
            // If hit...
                break;
            case AISettings.Behaviour.Skittish:                
                task = FLEE_FROM_POSITION;                
                break;
            case AISettings.Behaviour.Neutual:
            // If hit...
                break;
            case AISettings.Behaviour.Hostile:
                task = new BTSelector(new List<BTNode>{ATTACK_TARGET, CHASE}); 
                break;
        }
        return task;
    }

    BTNode GenerateTask(Inventory_ItemStats item){

        BTSlotsAvailable STILL_HAS_SLOTS = new BTSlotsAvailable(inventory);
        BTCanHoldWeapon CAN_HOLD_WEAPON = new BTCanHoldWeapon(ai, inventory, item);
        BTMove MOVE_TO_ITEM = new BTMove(ai, item.transform.position, ai.agentToTargetDelta);
        BTPickUpItem PICK_UP_ITEM = new BTPickUpItem(ai, inventory, item);
        BTWait WAIT = new BTWait(ai, .25f);

        BTNode subtasks = null;
        if(item.GetType() == typeof(Inventory_AmmunitionStats)){
            BTNeedsAmmo NEED_AMMO = new BTNeedsAmmo(ai, inventory, item); // Check if ammo is of use to this agent
            subtasks = new BTSequence(new List<BTNode>{STILL_HAS_SLOTS, NEED_AMMO, MOVE_TO_ITEM});
        }
        else if(item.GetType() == typeof(Inventory_WeaponStats)){
            BTDropItem DROP_ITEM = new BTDropItem(ai, inventory, item);        
            BTSelector PRE_TASK = new BTSelector(new List<BTNode>{DROP_ITEM, MOVE_TO_ITEM});    
            subtasks = new BTSequence(new List<BTNode>{STILL_HAS_SLOTS, CAN_HOLD_WEAPON, PRE_TASK});
        }

        BTSequence GET_ITEM = new BTSequence(new List<BTNode>{subtasks, PICK_UP_ITEM, WAIT});
        return GET_ITEM;
    }

    /// <summary>
    /// /// This function will add a task to memory for processing by the agent in the future
    /// </summary>
    /// <param name="transform">This is the object the task will be assigned around</param>
    /// <param name="task">This is the task which will drive the behaviour</param>
    int? AddTaskToMemory(float? taskLifetime, float priority, AISettings.TaskPriorities taskPriority, Transform transform, BTNode task){
        if(task == null){ return null; } // If task holds no value...        
        // Create new memory data...
        memory.activeMemory.Add(
            new AIMemory.Memory(
                taskLifetime,
                priority,
                taskPriority,
                transform.position,
                transform,
                task
            )
        );
        return memory.activeMemory.Count;
    }

    /// <summary>
    /// This function will check all targets scanned in the vision of the agent... If a target is a match to a condition, then it will be added as a task in memory
    /// </summary>
    /// <returns>A running NodeState so that other nodes can be ran simultaneously</returns>
    public override NodeState Evaluate(){  
        foreach(Transform transform in ai.entitiesInSight){
            CharacterBase character = transform.GetComponent<CharacterBase>();
            AIBrain entityAI = character?.GetComponent<AIBrain>(); // If character exists... Will check to see if AIBrain exists        
            Inventory_ItemStats item = transform.GetComponent<Inventory_ItemStats>();
            
            if(character){ // If the transform is a character...
                // Check to see if character is found inside the behaviours                
                foreach(AISettings.Behaviours behaviour in ai.agentProfile.behaviours){                    
                    bool isPlayer = (character.tag == behaviour.playerTag);
                    bool charactersFound = false;
                    // Ensure that the target is meant to be searched for
                    foreach(AISettings aiSettings in behaviour.targetsToAttach){                                            
                        if(entityAI && aiSettings.name == entityAI.agentProfile.name){ 
                            charactersFound = true; 
                            break;
                        }                        
                    }                    
                    if(!(isPlayer || (charactersFound && entityAI && entityAI.agentProfile.name != ai.agentProfile.name))){ // If there is no player or characters found...
                        continue;
                    }   
                    
                    // GET TASK SETTINGS
                    float? taskLifetime = null;
                    float taskPriority = 0;
                    foreach(AISettings.Priorities tPriority in ai.agentProfile.priorities){
                        if(tPriority.task == AISettings.TaskPriorities.Target){ 
                            taskLifetime = tPriority.timeInMemory;
                            taskPriority = tPriority.priority;
                        }
                    }

                    // CHECK FOR EXISTING MEMORY DATA
                    bool dataExists = false;
                    /// <summary>
                    /// This for each function will set 'dataExists' to true IF it has found a memory value which matches a value inside the sight of the ai...
                    /// </summary>                    
                    memory.activeMemory.ForEach(data => {
                        if(data.recordedTarget == transform && data.taskPriority == AISettings.TaskPriorities.Target){ 
                            data.lifetime = taskLifetime; // Updates the lifetime of the task
                            dataExists = true;                             
                        }
                    });          

                       
                    if(dataExists){ continue; } // If this data already exists... Skip; No point creating a new memory value for this data if it already exists... Simply update it
                    // Create a task based on its behaviour                    
                    AddTaskToMemory(taskLifetime, taskPriority, AISettings.TaskPriorities.Target, transform, GenerateTask(transform, behaviour.behaviourFor));                          
                }
            }            
            else if(item && item.itemSettings.isLootable){
                // GET TASK SETTINGS
                float? taskLifetime = null;
                float taskPriority = 0;
                foreach(AISettings.Priorities tPriority in ai.agentProfile.priorities){
                    if(tPriority.task == AISettings.TaskPriorities.Resourcefulness){ 
                        taskLifetime = tPriority.timeInMemory;
                        taskPriority = tPriority.priority;
                    }
                }

                // CHECK FOR EXISTING MEMORY DATA
                bool dataExists = false;
                /// <summary>
                /// This for each function will set 'dataExists' to true IF it has found a memory value which matches a value inside the sight of the ai...
                /// </summary>                    
                memory.activeMemory.ForEach(data => {
                    if(data.recordedTarget == transform && data.taskPriority == AISettings.TaskPriorities.Resourcefulness){ 
                        data.lifetime = taskLifetime; // Updates the lifetime of the task
                        dataExists = true;                             
                    }
                }); 
                if(dataExists){ continue; } // If this data already exists... Skip; No point creating a new memory value for this data if it already exists... Simply update it
                // Create a task based on its behaviour       
                if(inventory.GetData(inventory.tag).Count >= inventory.Slots){ continue; }   

                if(item.GetType() == typeof(Inventory_WeaponStats)){
                    BTCanHoldWeapon canHoldWeapon = new BTCanHoldWeapon(ai, inventory, item, true);
                    if(canHoldWeapon.Evaluate() == NodeState.Fail){
                        return NodeState.Success;
                    }
                }
                AddTaskToMemory(taskLifetime, taskPriority, AISettings.TaskPriorities.Resourcefulness, transform, GenerateTask(item));            
            }
        }        
        return NodeState.Success;
    }
}
