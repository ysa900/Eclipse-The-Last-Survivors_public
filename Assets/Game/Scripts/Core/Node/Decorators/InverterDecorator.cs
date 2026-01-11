namespace Eclipse
{
    public class InverterDecorator : Node
    {
        protected Node _child;
        public Node Child { get { return _child; } }

        public InverterDecorator(Node child)
        {
            _child = child;
        }

        public override NodeState Evaluate()
        {
            switch (_child.Evaluate())
            {
                case NodeState.Success:
                    return NodeState.Failure;
                case NodeState.Failure:
                    return NodeState.Success;
                case NodeState.Running:
                    return NodeState.Running;
            }
            return NodeState.Failure;
        }
    }
}