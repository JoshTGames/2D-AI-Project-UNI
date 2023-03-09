
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---
    This script only touches the interest weights. avoid weights are handled in other scripts.
*/

namespace JoshGames.AI.BehaviourTree{
    public abstract class BTNode{
        protected NodeState state; // This is the active state this node is.
        public NodeState State{get { return state; } } // This is a public read-only value which can be read from from a behaviour tree or for some other reason, another script. 

        /// <summary>
        /// This function could be anything. It will be a task that a behaviour tree will iterate through at some point. But it must return an enum value either Success, Fail or Running.
        /// </summary>
        /// <returns>A nodestate being either Success, Fail or Running.</returns>
        public abstract NodeState Evaluate();
    }
    
    // These are all the potential states of a given node.
    public enum NodeState{
            Fail,
            Success,
            Running
    }
}