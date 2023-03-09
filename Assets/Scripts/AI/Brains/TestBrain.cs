using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.Character;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Steering;
using UnityEngine.Tilemaps;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
*/

public class TestBrain : AIBrain{      
    protected override BTNode CreateTree(){     
        /* NOTES - How this agent works abstractly... :
        * SCOUTING - This node set is for when there is no data found inside the senses... So it falls back into a wandering state        
        * PERFORMING - This node set is all about performing upon the data given by the senses... It'll perform the task of highest priority first
        */

        //-- DECISION TREE BUILDER
            BTWander WANDER = new BTWander(this, 2f, scanTreeFrequency, 1f, 10f, transform.position);
            BTPatrol PATROL = new BTPatrol(this, scanTreeFrequency, 1f, 10f);            
        BTSelector SCOUTING = new BTSelector(new List<BTNode>{PATROL, WANDER}); 
                
        //     BTSequence HEARING = new BTSequence(new List<BTNode>{});                
        //     BTSequence SIGHT = new BTSequence(new List<BTNode>{TARGET_IN_SIGHT, ADD_TO_MEMORY});
        // BTSelector SENSING = new BTSelector(new List<BTNode>{SIGHT});
            BTMemoryProcessor PROCESSOR = new BTMemoryProcessor(memory);
        BTSelector PERFORMING = new BTSelector(new List<BTNode>{PROCESSOR});
        //--
        BTSelector ROOT_NODE = new BTSelector(new List<BTNode>{PERFORMING, SCOUTING}); // This is the root node and will be iterated through to get behaviours result from it.
        return ROOT_NODE;
    }
    
    
    [SerializeField] BTNode rootSensingNode;

    /// <summary>
    /// This function creates a behaviour tree, designed to read from sensors and create tasks based on inputs for the primary behaviour tree to perform.
    /// </summary>
    /// <returns>A root node which will be evaluated to find a task to be acted upon.</returns>
    protected BTNode CreateSensingTree(){
        BTSight SIGHT = new BTSight(this, memory, inventory);
        BTHearing HEARING = new BTHearing(this, memory);
        return new BTSequence(new List<BTNode>(){SIGHT, HEARING});
    }

    /// <summary>
    /// This function is what actually iterates through all the nodes. 
    /// </summary>
    void ScanSensingTree(){
        if(rootSensingNode == null){ 
            Debug.LogWarning($"{transform.name} {this.name} script has a null rootSensingNode... Please fix this for this script to operate!");
            return; 
        }
        switch(rootSensingNode.Evaluate()){
            case NodeState.Success:
                break;
            case NodeState.Fail:
                break;
            case NodeState.Running:
                break;
        }
    }

    protected override void Start() {
        base.Start();
        bounds = (bounds)? bounds : GameObject.Find("Grass").GetComponent<Tilemap>();
        // DISCLAIMER: These steering behaviours will have to be made a dynamic solution as some behaviours such as seek may need to be modified to seek various different things...
        // Set up avoid behaviour
        SteeringAvoidance avoid = new SteeringAvoidance();
        avoid.transform = transform;
        steeringBehaviours.Add(avoid);

        // Set up seek behaviour
        SteeringSeek seek = new SteeringSeek();
        seek.transform = transform;        
        steeringBehaviours.Add(seek);     

        rootSensingNode = CreateSensingTree();
        InvokeRepeating("ScanSensingTree", 0f, scanTreeFrequency);     
    }   

    private void FixedUpdate() {              
        Move(moveDirection, profile.speed, 0);
    }    
}
