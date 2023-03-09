
/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2022-2023 Joshua Thompson. All Rights Reserved. ---    
*/

namespace JoshGames.AI.BehaviourTree{
    public class BTInverter : BTNode{
        protected BTNode node;
        public BTInverter(BTNode _node){ this.node = _node; }

        public override NodeState Evaluate(){
            switch(node.Evaluate()){
                case NodeState.Running:                        
                    return NodeState.Running;
                case NodeState.Success:
                    return NodeState.Fail;
                case NodeState.Fail:                        
                    return NodeState.Success;                   
                default:
                    break;
            }
            return NodeState.Fail;
        }
    }
}