using System;

namespace Eclipse
{
    public class ActionTask : Node
    {
        private Action action;

        public ActionTask(Action action, string name = "")
        {
            this.action = action;
            this.name = name;
        }

        public override NodeState Evaluate()
        {
            action();
            state = NodeState.Success;
            return state;
        }
    }
}