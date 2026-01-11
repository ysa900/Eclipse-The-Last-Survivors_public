using System;
using UnityEngine;

namespace Eclipse
{
    public class CheckTask<T> : Node where T : IComparable<T>
    {
        private T threshold; // 임계값
        private Func<T> getData;

        public CheckTask(Func<T> getData, T threshold, string name = "")
        {
            this.getData = getData;
            this.threshold = threshold;
            this.name = name;
        }

        public override NodeState Evaluate()
        {
            // getData() 값이 threshold 이하인지 비교
            state = getData().CompareTo(threshold) <= 0 ? NodeState.Success : NodeState.Failure;
            return state;
        }
    }
}
