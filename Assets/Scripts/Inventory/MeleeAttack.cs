using System.Collections;
using System.Collections.Generic;
using JoshGames.CharacterController;
using JoshGames.AI.Character;
using UnityEngine;

namespace JoshGames.ItemStorage{
    public class MeleeAttack : MonoBehaviour{
        [SerializeField] Inventory_WeaponStats weapon;    

        public bool activateThis;    
        public AudioSource audioSource;
        private void OnTriggerEnter2D(Collider2D other) {      
            if(!weapon.Attack || other.transform.GetInstanceID() == weapon.owner.transform.GetInstanceID()){ return; } // stops unneeded calls and stops hitting itself
            if(!activateThis){ return; } // If multiple attacking objects. This object in particular may not have been called upon...

            weapon.Attack = false;            
            CharacterBase eChar = other.transform.GetComponent<CharacterBase>();
            eChar = (eChar)? eChar : other.transform.parent.GetComponent<CharacterBase>(); 
            AIBrain aI = weapon.owner.GetComponent<AIBrain>();
            AIBrain eAI = eChar?.transform.GetComponent<AIBrain>();

            if((aI && eAI && aI.agentProfile.name == eAI.agentProfile.name)){ return; } // Means that this character profile, cant attack another character of same profile.
            
            // DO stuff e.g. Deal damage
            eChar?.Damage(weapon.thisItem.weaponData.weaponDamage, weapon.owner.transform.position);
        }

        private void Awake() {
            weapon = (weapon)? weapon : GetComponent<Inventory_WeaponStats>();
        }
    }
}