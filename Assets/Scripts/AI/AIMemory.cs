using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using System;
using UnityEngine;
using JoshGames.AI.BehaviourTree;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.AI.Character{
    public class AIMemory : MonoBehaviour{
        [Tooltip("Greater the value, the longer it takes to deduct a time off the data")][SerializeField] float memoryCheckFrequency = 1f; 
        public List<Memory> activeMemory = new List<Memory>(); // (MAIN MEMORY)

        public class Memory{                  
            public float? lifetime;
            public float? taskLifeTime;
            public float priority;
            public AISettings.TaskPriorities taskPriority;
            public Vector2 recordedPosition;
            public Transform recordedTarget;
            public BTNode task;


            public Memory(float? _lifetime, float _priority, AISettings.TaskPriorities _taskPriority, Vector2 _position, Transform _target = null, BTNode _task = null){                
                this.taskLifeTime = _lifetime;
                this.lifetime = _lifetime;
                this.priority = _priority;
                this.taskPriority = _taskPriority;
                this.recordedPosition = _position;
                this.recordedTarget = _target;
                this.task = _task;                
            }
        }

        /// <summary>
        /// This will call a function every x amount of seconds
        /// </summary>
        private void Start() => InvokeRepeating("CheckMemory", 0f, memoryCheckFrequency);

        /// <summary>
        /// This function will check the memory dictionary and compare the float value attached to each key... If value is <=0 then it will be removed... If it is null... Then it is being used
        /// </summary>
        void CheckMemory(){
            for(int i = 0; i < activeMemory.Count; i++){
                activeMemory[i].lifetime -= memoryCheckFrequency; // Negates x from the duration of the memory
                if(activeMemory[i].lifetime <= 0){ activeMemory.RemoveAt(i); } // Delete old data... (If value is a numerical and less or equal to 0...)
                
            }            
        }        
    }
}