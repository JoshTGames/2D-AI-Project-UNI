using System.Collections.Generic;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

namespace JoshGames.AI.BehaviourTree{
    public class BTSequence : BTNode{
        protected List<BTNode> nodes = new List<BTNode>();

        public BTSequence(List<BTNode> _nodes){ this.nodes = _nodes; }
        public override NodeState Evaluate(){
            bool isAnyRunning = false;
            foreach(BTNode node in nodes){
                switch(node.Evaluate()){
                    case NodeState.Running:
                        isAnyRunning = true;
                        break;
                    case NodeState.Success:
                        break;
                    case NodeState.Fail:                        
                        return NodeState.Fail;                        
                    default:
                        break;
                }
            }            
            return (isAnyRunning)? NodeState.Running : NodeState.Success;
        }
    }
}