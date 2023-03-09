using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.AI.Sensors;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTHearing : BTNode{
    AIBrain ai;
    AIMemory memory; 
    public BTHearing(AIBrain _ai, AIMemory _memory){ 
        this.ai = _ai;
        this.memory = _memory;        
    }

        /// <summary>
    /// This function will generate a task to be performed later by the AI
    /// </summary>
    /// <param name="position">This is the position which will be used to calculate whether or not this character will face the position or move towards it</param>
    /// <param name="behaviour">This will dictate what task is generated</param>
    /// <returns>The task to be soon added to memory</returns>
    BTNode GenerateTask(SoundEmitter.Sound sound, float curiosity, AISettings.Behaviour behaviour){
        BTNode task = null;

        BTLookAt LOOK_AT = new BTLookAt(ai, sound.soundPosition);
        BTMove MOVE_TO_NOISE = new BTMove(ai, sound.soundPosition);

        BTIsAtPosition IS_AT_POSITION = new BTIsAtPosition(ai, sound.soundPosition);
        BTWait WAIT = new BTWait(ai, .25f);        

        int rndInvestigateVal = Random.Range(ai.agentProfile.directionsToInvestigate.min, ai.agentProfile.directionsToInvestigate.max);
        BTInvestigate INVESTIGATE = new BTInvestigate(ai, sound.soundRadius * .1f, sound.soundRadius, .25f, .75f, rndInvestigateVal, sound.soundPosition);

        BTSequence FACE_TARGET = new BTSequence(new List<BTNode>{LOOK_AT, WAIT});
        BTSequence MOVE_TO_TARGET = new BTSequence(new List<BTNode>{MOVE_TO_NOISE, WAIT});
        
        switch(behaviour){
            case AISettings.Behaviour.Passive:
                // Nothing...
                break;
            case AISettings.Behaviour.Skittish:                
                // Face noise direction...
                if(curiosity > 0.2f){
                    task = FACE_TARGET;
                } 
                break;
            case AISettings.Behaviour.Neutual:            
                // Nothing...
                break;
            case AISettings.Behaviour.Hostile:                                      
                if(curiosity > 0.5f){ task = new BTSequence(new List<BTNode>{FACE_TARGET, MOVE_TO_TARGET, INVESTIGATE}); } // Investigate noise IF curiosity is high enough otherwise, simply just face noise direction      
                else if(curiosity > 0.2f){ task = FACE_TARGET; }      
                
                // task = new BTSequence(new List<BTNode>{LOOK_AT /*, MOVE_TO_NOISE -- This behaviour will move to noise if curiosity is high enough*/, WAIT});
                break;
        }
        return task;
    }

    /// <summary>
    /// This function will add a task to memory for processing by the agent in the future
    /// </summary>
    /// <param name="transform">This is the object the task will be assigned around</param>
    /// <param name="task">This is the task which will drive the behaviour</param>
    int? AddTaskToMemory(float? taskLifetime, float priority, AISettings.TaskPriorities taskPriority, SoundEmitter.Sound sound, BTNode task){
        if(task == null){ return null; } // If task holds no value...        
        // Create new memory data...
        memory.activeMemory.Add(
            new AIMemory.Memory(
                taskLifetime,
                priority,
                taskPriority,
                sound.soundPosition,
                (sound.soundEmitter)? sound.soundEmitter?.transform : null,
                task
            )
        );
        return memory.activeMemory.Count;
    }

    float Remap(float inputValue, float fromMin, float fromMax, float toMin, float toMax){
        float i = (((inputValue - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin);
        i = Mathf.Clamp(i, toMin, toMax);
        return i;
    }


    float CalculateCuriosity(int _noise){
        SoundEmitter.Sound noiseObj = SoundEmitter.instance.worldSounds[_noise];
        float distanceFromSound = Vector3.Distance(noiseObj.soundPosition, ai.transform.position);
        float distancePower = 1 - Remap(distanceFromSound, 0, noiseObj.soundRadius, 0, 1);        
                
        // Curiosity will be incremented by how long they are within the radius of the sound and slightly by how close they are to the sound as this could cause problems if this is a dominant value.
        // This value will be used to decide whether or not this agent will simply look towards the target
        return distancePower;
    }

    public override NodeState Evaluate(){          
        SoundEmitter noiseInstance = SoundEmitter.instance;
        int? noiseIndex = noiseInstance.GetClosestSound(ai.transform.position, ai);
        if(noiseIndex == null){ return NodeState.Fail; }
        int noise = (int) noiseIndex;
        SoundEmitter.Sound noiseObj = noiseInstance.worldSounds[noise];

        // Get dominant behaviour...
        AISettings.Behaviour dominantBehaviour = AISettings.Behaviour.Passive;
        int topValue = 0;
        foreach(AISettings.Behaviours behaviourData in ai.agentProfile.behaviours){
            AISettings.Behaviour behaviour = behaviourData.behaviourFor;
            int thisValue = 0;
            switch(behaviour){
                case AISettings.Behaviour.Passive:
                    thisValue = 1;
                    break;
                case AISettings.Behaviour.Skittish:
                    thisValue = 2;
                    break;
                case AISettings.Behaviour.Neutual:
                    thisValue = 3;
                    break;
                case AISettings.Behaviour.Hostile:
                    thisValue = 4;
                    break;
            }

            if(thisValue > topValue){
                topValue = thisValue;
                dominantBehaviour = behaviour;
            }
        }        
        
        // GET TASK SETTINGS
        float? taskLifetime = null;
        float taskPriority = 0;
        foreach(AISettings.Priorities tPriority in ai.agentProfile.priorities){
            if(tPriority.task == AISettings.TaskPriorities.Position){ 
                taskLifetime = tPriority.timeInMemory;
                taskPriority = tPriority.priority;                
            }
        }

        // CHECK FOR EXISTING MEMORY DATA        
        /// <summary>
        /// This for each function will set 'dataExists' to true IF it has found a memory value which matches a value inside the sight of the ai...
        /// </summary>       
        for(int i = 0; i < memory.activeMemory.Count; i++){
            AIMemory.Memory data = memory.activeMemory[i];
            if(data?.recordedTarget == noiseObj.soundEmitterTransform && data.taskPriority == AISettings.TaskPriorities.Position){                 
                memory.activeMemory.RemoveAt(i); 
            }    
        }           
        


        // Create a curiosity metre which will calculate a value which is compared to a threshold to either simply look at the direction the sound is coming from OR moving towards the sound.
        AddTaskToMemory(taskLifetime, taskPriority, AISettings.TaskPriorities.Position, noiseObj, GenerateTask(noiseObj, CalculateCuriosity(noise), dominantBehaviour));       
        return NodeState.Success;
    }
}
