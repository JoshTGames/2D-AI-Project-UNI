using UnityEngine;
using JoshGames.AI.BehaviourTree;
using JoshGames.AI.Character;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
        This class will return TRUE if the chance value is less than the result of the health calculation.
        It works by multiplying the max health by a variable which dictates what low health of an agent is. Once the health enters 'low health', Thats when the possibility of fleeing comes into play!
        This class also means, that it is almost certain at some point, it will always return true. It just depends on the chance value.
*/

public class BTDiceRollHealth : BTNode{
    float chance;

    AIBrain ai;
    float health, maxHealth;
    float allowFleeAtPercentHealth;
    public BTDiceRollHealth(AIBrain _ai = null, float minChance = 0, float maxChance = 100){         
        this.ai = _ai;
        this.allowFleeAtPercentHealth = _ai.agentProfile.allowFleeAtPercentHealth;
        this.chance = Mathf.Clamp((float)Random.Range(0, 100), minChance, maxChance)/100;
    }   
    
    public override NodeState Evaluate(){ 
        ai.GetHealth(out health, out maxHealth);

        float healthPercentage = 1 - (health / (maxHealth * (allowFleeAtPercentHealth / 100))); // If health is 5, maxhealth is 10 and lowHealthPercentage is 0.5. Then the result will be 1 aka 100%       
        return (chance < healthPercentage)? NodeState.Success : NodeState.Fail;
    }
}
