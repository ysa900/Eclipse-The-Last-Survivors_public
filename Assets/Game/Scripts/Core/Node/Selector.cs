using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse
{
    public class Selector : Node
    {
        private List<Node> nodes;

        // »ý¼ºÀÚ
        public Selector(List<Node> nodes, string name = "") 
        { 
            this.nodes = nodes; 
            this.name = name;
        }

        public override NodeState Evaluate()
        {
            foreach (var node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Success:
                        state = NodeState.Success;
                        return state;
                    case NodeState.Running:
                        state = NodeState.Running;
                        return state;
                }
            }
            state = NodeState.Failure;
            return state;
        }
    }
}