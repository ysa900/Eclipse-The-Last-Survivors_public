using UnityEngine;

namespace Eclipse
{
    public abstract class Node
    {
        public enum NodeState { Undefined = 0, Running, Success, Failure }
        protected NodeState state;
        public string name; // 디버깅용 변수

        public NodeState State => state;
        public abstract NodeState Evaluate();
    }
}