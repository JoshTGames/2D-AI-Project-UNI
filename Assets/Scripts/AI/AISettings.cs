using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JoshGames.ItemStorage;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.AI.Character{
    [CreateAssetMenu(fileName = "AI Profile", menuName = "ScriptableObjects/AI/Create Profile")]
    public class AISettings : ScriptableObject{
        
        public float maxHealth, health, speed;
        [Tooltip("This will replace the equipped item slot to stop the value being null.")] public Item[] defaultHeldItems;
        [Tooltip("All layermasks in here will be checked... If an entity is found, it will be processed to see if its valuable to the agent or not.")] public LayerMask entitiesOfInterest;
        [Tooltip("Anything inside this layer will be ignored when firing raycasts.")] public LayerMask ignoreInteractionLayer;
        public Behaviours[] behaviours; // This is a basic personality system... It will allow me to define what an agent will do based on what targets are in its proximity
        public Priorities[] priorities; // This will be used when calculating which tasks should be carried out first... Higher priority means it'll be carried out before others
        public GameObject blood, deathFX;
        [Tooltip("The max amount of weaponry this agent will want to carry at a time")] public int maxWeaponaryThisCharacterCanHold;
        public MinMax directionsToInvestigate;
        [Range(0, 1)] public float chanceToInvestigate = 1;
        [Header("(FOR HOSTILE CHARACTERS): Will allow chance to flee when health is below this value")]
        [Range(0, 100)] public float allowFleeAtPercentHealth = 25;

        [Serializable] public class MinMax{ public int min = 1, max = 3; }

        [Serializable] public struct Behaviours{
            [Tooltip("The agent will inherit this behaviour to given ")]public Behaviour behaviourFor;
            
            [Header("Attach behaviour to these targets...")]
            [Tooltip("Leave this empty if you dont want the player to be attached to this behaviour")] public string playerTag;
            public AISettings[] targetsToAttach;
        }
        [Serializable] public struct Priorities{
            [Tooltip("This will be used in memory to figure out which tasks will be completed first")] public TaskPriorities task;
            [Tooltip("This is the weight assigned to this specific task")] [Range(0, 1)] public float priority;
            [Tooltip("This will dictate how long this type of task will be lingering in memory...")] public float timeInMemory;
        }
        [Serializable] public enum Behaviour{
            Passive, // Flees only when attacked...
            Skittish, // Flees from any character but of its own kind...
            Neutual, // Attacks when attacked...
            Hostile // Attacks characters...
        }
        [Serializable] public enum TaskPriorities{
            [Tooltip("This enum does nothing...")]                                                                     None,
            [Tooltip("How important is it for this agent to react to targets in this agents proximity?")]              Target,
            [Tooltip("How important is it for this agent to do \"Resourceful\" tasks?")]                               Resourcefulness,
            [Tooltip("How important is it for this agent to move to a position such as for when it hears something?")] Position
        }
    }
}