using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using System.Linq;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class BTMemoryProcessor : BTNode{
    AIMemory memory;

    AIMemory.Memory currentTask;
    public BTMemoryProcessor(AIMemory _memory){
        this.memory = _memory;
    }

    /// <summary>
    /// This function finds a task with the highest priority and chooses to run that task.
    /// </summary>
    /// <returns>Whether or not the task was do-able.</returns>
    public override NodeState Evaluate(){ 
        if(!memory || memory.activeMemory.Count <= 0){ return NodeState.Fail; } // If there is no data to act upon... 
            
        currentTask = memory.activeMemory.OrderByDescending(task => task.priority).ToList()[0];
        // If task is a target task... Then find closest target
        if(currentTask.taskPriority == AISettings.TaskPriorities.Target){
            List<int> targets = new List<int>();

            for(int i = 0; i < memory.activeMemory.Count; i++){
                AIMemory.Memory data = memory.activeMemory[i];
                if(data.taskPriority == AISettings.TaskPriorities.Target && data.recordedTarget){ targets.Add(i); }
            }
            if(targets.Count> 0){
                currentTask = memory.activeMemory[targets.OrderBy((target) => (Vector3.Distance(memory.activeMemory[target].recordedTarget.position, memory.transform.position))).ToList()[0]];
            }
        }
        
        memory.activeMemory.ForEach(data =>{
            if(data.recordedTarget == currentTask.recordedTarget && data.taskPriority == currentTask.taskPriority){
                data.lifetime = data.taskLifeTime;
            }
        });
        NodeState taskState = currentTask.task.Evaluate();        
        if(taskState == NodeState.Success){ memory.activeMemory.Remove(currentTask); }

        return (taskState == NodeState.Success)? NodeState.Success : NodeState.Running;
    }
}
