using System.Collections.Generic;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

namespace JoshGames.AI.BehaviourTree{
    public class BTSelector : BTNode{
        protected List<BTNode> nodes = new List<BTNode>();

        public BTSelector(List<BTNode> _nodes){ this.nodes = _nodes; }
        public override NodeState Evaluate(){
            
            foreach(BTNode node in nodes){
                switch(node.Evaluate()){
                    case NodeState.Running:                        
                        return NodeState.Running;
                    case NodeState.Success:
                        return NodeState.Success;
                    case NodeState.Fail:                        
                        break;                      
                    default:
                        break;
                }
            }            
            return NodeState.Fail;
        }
    }
}