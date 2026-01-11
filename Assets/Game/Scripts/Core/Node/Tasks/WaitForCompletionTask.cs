using System;
using System.Collections;
using UnityEngine;

namespace Eclipse
{
    public class WaitForCompletionTask : Node
    {
        private Func<IEnumerator> action; // 실행할 비동기 작업을 반환하는 함수
        private MonoBehaviour monoBehaviour; // 코루틴을 실행할 MonoBehaviour
        private bool isRunning = false; // 작업이 실행 중인지 여부
        private bool isCompleted = false; // 작업이 완료되었는지 여부
        private Func<bool> shouldRetryTask; // 작업 재실행이 필요한지 여부

        public WaitForCompletionTask(MonoBehaviour monoBehaviour, Func<IEnumerator> action, Func<bool> shouldRetryTask, string name = "")
        {
            this.monoBehaviour = monoBehaviour;
            this.action = action;
            this.name = name;
            this.shouldRetryTask = shouldRetryTask;
        }

        public override NodeState Evaluate()
        {
            /*switch (name)
            {
                case "MoveForSec_1":
                    Debug.Log("MoveForSec_1 Evaluate called"); Debug.Log($"shouldRetryTask: {shouldRetryTask()}");
                    break;
                case "MoveForSec_2":
                    Debug.Log("MoveForSec_2 Evaluate called"); Debug.Log($"shouldRetryTask: {shouldRetryTask()}");
                    break;
                case "MoveForSec_3":
                    Debug.Log("MoveForSec_3 Evaluate called"); Debug.Log($"shouldRetryTask: {shouldRetryTask()}");
                    break;
                case "MoveForSec_4":
                    Debug.Log("MoveForSec_4 Evaluate called"); Debug.Log($"shouldRetryTask: {shouldRetryTask()}");
                    break;
                case "MoveForSec_5":
                    Debug.Log("MoveForSec_5 Evaluate called"); Debug.Log($"shouldRetryTask: {shouldRetryTask()}");
                    break;
            }*/
            // 작업이 이미 완료되었고, 재실행이 필요하지 않으면 Success 반환
            if (isCompleted && !shouldRetryTask())
                return NodeState.Success;

            // 작업이 실행 중이면 Running 반환
            if (isRunning)
                return NodeState.Running;

            // 작업이 실행 중이 아니면 작업 시작
            monoBehaviour.StartCoroutine(ExecuteAction());
            return NodeState.Running; // 첫 Evaluate에서는 Running 반환
        }

        private IEnumerator ExecuteAction()
        {
            isRunning = true;
            yield return action.Invoke(); // 비동기 작업 실행

            isRunning = false;
            isCompleted = true; // 작업이 완료되면 완료 상태로 설정
        }
    }
}
