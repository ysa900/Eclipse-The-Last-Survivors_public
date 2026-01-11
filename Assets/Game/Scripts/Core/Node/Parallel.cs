using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse
{
    public class Parallel : Node
    {
        private List<Node> childNodes;
        private int successThreshold;  // Success를 반환하는 데 필요한 성공 수

        public Parallel(List<Node> childNodes, int successThreshold = -1)
        {
            this.childNodes = childNodes;
            this.successThreshold = successThreshold < 0 ? childNodes.Count : successThreshold; // 기본값은 전체 성공
        }

        public override NodeState Evaluate()
        {
            int successCount = 0;
            int failureCount = 0;

            foreach (Node child in childNodes)
            {
                NodeState result = child.Evaluate();
                if (result == NodeState.Success)
                {
                    successCount++;
                }
                else if (result == NodeState.Failure) 
                { 
                    failureCount++; 
                }
            }

            // 성공 횟수를 충족했으면
            if (successCount >= successThreshold)
            {
                state = NodeState.Success;
            }
            // 전부 다 실패했으면
            else if (failureCount == childNodes.Count)
            {
                state = NodeState.Failure;
            }
            // 아직 실행중인 노드가 있으면
            else
            {
                state = NodeState.Running;
            }

            return state;
        }
    }

}