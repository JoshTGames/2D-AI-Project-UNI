using System.Collections.Generic;
using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;
using JoshGames.ItemStorage;
using JoshGames.CharacterController;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

public class BTAttack : BTNode{
    AIBrain aI;       
    Inventory inv;
    Transform target;

    public BTAttack(AIBrain _ai, Inventory _inv, Transform _target){ 
        this.aI = _ai;
        this.inv = _inv;
        this.target = _target;
    }
    
    public override NodeState Evaluate(){     
        if(!(target && (target.position - aI.transform.position).magnitude <= aI.EquippedItem.weaponData.sightRange)){ return NodeState.Fail; } // If target is greater than weapon range... return

        bool found = false;
        if(Vector3.Distance(aI.vision.transform.position, target.position) > aI.vision.transform.localPosition.magnitude * 2.5f){ // Makes sure the target is visible
            foreach(Transform entity in aI.entitiesInSight){
                if(entity.transform.name != target.name){ continue; }
                found = true;
            }
            if(!found){ return NodeState.Fail; }
        }
        
        // Check if weapon is facing target before firing ranged weapon...
        Transform weapon = aI.GetEquippedItemVisual().transform;
        Vector3 delta = target.position - weapon.position;
        float angle = Vector3.Angle(delta, weapon.transform.right);
        if(angle > 7 && aI.EquippedItem.weaponData.projectile){ return NodeState.Success; }

        aI.positionOfInterest = target.position;
        aI.moveDirection = Vector3.zero;
        aI.UseHeldObject(aI.GetEquippedItemVisual());
        return NodeState.Success;
    }
}
