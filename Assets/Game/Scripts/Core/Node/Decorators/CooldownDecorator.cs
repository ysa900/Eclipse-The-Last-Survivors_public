using System;
using UnityEngine;

namespace Eclipse
{
    public class CooldownDecorator : Node
    {
        private Node childNode;
        private Func<float> getCoolDown; // 쿨타임 데이터 가져오는 함수
        private float lastExecutionTime; // 마지막 실행 시간

        public CooldownDecorator(Node node, Func<float> getCoolDown, string name = "")
        {
            childNode = node;
            this.getCoolDown = getCoolDown;
            this.name = name;
        }

        public override NodeState Evaluate()
        {
            if (Time.time - lastExecutionTime >= getCoolDown())
            {
                lastExecutionTime = Time.time; // 마지막 실행 시간을 현재 시간으로 업데이트
                return childNode.Evaluate(); // 쿨다운 시간 이상일 때만 자식 노드 실행
            }

            return NodeState.Failure; // 쿨다운 중일 때는 Failure 반환
        }
    }
}
