using System.Collections.Generic;
using UnityEngine;

namespace Eclipse
{
    public class Sequence : Node
    {
        private List<Node> nodes;

        public Sequence(List<Node> nodes, string name = "") 
        { 
            this.nodes = nodes; 
            this.name = name;
        }

        public override NodeState Evaluate()
        {
            bool anyNodeRunning = false;
            foreach (var node in nodes)
            {
                NodeState result = node.Evaluate();
                switch (result)
                {
                    case NodeState.Failure:
                        state = NodeState.Failure;
                        return state;
                    case NodeState.Running:
                        anyNodeRunning = true;
                        break;
                }
            }
            state = anyNodeRunning ? NodeState.Running : NodeState.Success;
            return state;
        }
    }
}