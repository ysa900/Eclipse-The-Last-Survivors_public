using System;

namespace Eclipse
{
    public class BoolCheckTask : Node
    {
        private bool threshold; // 임계값
        private Func<bool> getData;

        public BoolCheckTask(Func<bool> getData, bool threshold, string name = "")
        {
            this.getData = getData;
            this.threshold = threshold;
            this.name = name;
        }

        public override NodeState Evaluate()
        {
            // getData() 값이 threshold 인지 판단
            state = getData() == threshold ? NodeState.Success : NodeState.Failure;

            return state;
        }
    }
}