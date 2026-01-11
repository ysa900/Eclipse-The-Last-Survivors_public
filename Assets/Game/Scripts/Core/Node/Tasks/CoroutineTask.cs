using System;
using System.Collections;
using UnityEngine;

namespace Eclipse
{
    public class CoroutineTask : Node
    {
        private Func<Action<bool>, IEnumerator> coroutineFunc; // 성공 여부 콜백을 받는 코루틴 함수
        private MonoBehaviour monoBehaviour; // 코루틴 실행 주체
        private Coroutine currentCoroutine; // 현재 실행 중인 코루틴

        public bool IsCoroutineRunning => state == NodeState.Running ? true : false;

        public CoroutineTask(MonoBehaviour monoBehaviour, Func<Action<bool>, IEnumerator> coroutineFunc, string name = "")
        {
            this.monoBehaviour = monoBehaviour;
            this.coroutineFunc = coroutineFunc;
            this.name = name;
        }

        public override NodeState Evaluate()
        {
            if (state == NodeState.Running)
            {
                return state; // 아직 진행 중이므로 바로 return
            }

            if (currentCoroutine == null)
            {
                // 처음 실행되거나, 코루틴 종료 후 다음 프레임이 지나면 이쪽으로 진입
                currentCoroutine = monoBehaviour.StartCoroutine(ExecuteCoroutine());
                state = NodeState.Running;
            }
            else if (state != NodeState.Running)
            {
                // 코루틴은 끝났고, 다음 프레임에 결과(state)는 Success 또는 Failure 반환
                currentCoroutine = null; // 한 프레임 지연 정리를 위해 여기서 null 처리
            }

            return state;
        }

        private IEnumerator ExecuteCoroutine()
        {
            // 코루틴이 끝날 때까지 대기
            yield return monoBehaviour.StartCoroutine(coroutineFunc.Invoke(OnCoroutineFinished));

            // 코루틴 본체에서 콜백 호출되지 않으면 강제로 false 처리
            if (state == NodeState.Running) // 콜백 호출 안 됐을 가능성
            {
                OnCoroutineFinished(false);
            }
        }

        private void OnCoroutineFinished(bool coroutineResult)
        {
            state = coroutineResult ? NodeState.Success : NodeState.Failure;
        }
    }
}