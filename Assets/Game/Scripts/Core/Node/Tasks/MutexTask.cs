using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse
{
    public class MutexTask : Node
    {
        private List<Node> childNodes; // 실행 가능한 자식 노드 목록
        private Node activeNode = null; // 현재 실행 중인 노드

        public MutexTask(List<Node> childNodes, string name = "")
        {
            this.childNodes = childNodes;
            this.name = name;
        }

        public override NodeState Evaluate()
        {
            // 현재 실행 중인 노드가 있다면 해당 노드만 평가
            if (activeNode != null)
            {
                NodeState result = activeNode.Evaluate();
                if (result == NodeState.Failure || result == NodeState.Success)
                {
                    // 실행이 끝난 경우, activeNode를 해제
                    activeNode = null;
                }
                return result;
            }

            // 실행 중인 노드가 없으면 자식 노드들 중 하나를 실행
            bool anyNodeRunning = false;
            foreach (Node node in childNodes)
            {
                NodeState result = node.Evaluate();
                if (result == NodeState.Running)
                {
                    activeNode = node; // 실행 중인 노드를 설정
                    return NodeState.Running;
                }
                else if (result == NodeState.Success)
                {
                    // Success 상태는 반환하지 않고 다음 자식 노드를 평가
                    anyNodeRunning = true; // 하나 이상의 성공적인 노드가 있었다는 표시
                }
            }

            // 모든 자식 노드를 평가했을 때 처리
            return anyNodeRunning ? NodeState.Success : NodeState.Failure;
        }

        public bool IsRunning()
        {
            // activeNode가 null이 아니면 실행 중
            return activeNode != null;
        }
    }
}
