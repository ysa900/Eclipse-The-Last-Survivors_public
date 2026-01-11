using System;

namespace Eclipse
{
    public class ConditionalDecorator : Node
    {
        private Func<bool> condition; // 조건을 확인하는 함수
        private Node childNode;       // 조건이 만족되었을 때 실행할 자식 노드

        public ConditionalDecorator(Func<bool> condition, Node childNode)
        {
            this.condition = condition;
            this.childNode = childNode;
        }

        public override NodeState Evaluate()
        {
            // 조건이 참인지 확인
            if (condition())
            {
                // 조건이 참이면 자식 노드를 평가
                return childNode.Evaluate();
            }

            // 조건이 거짓이면 Failure 반환
            return NodeState.Failure;
        }
    }
}
