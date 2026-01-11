using System.Collections;
using UnityEngine;

namespace Eclipse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Screen : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        [SerializeField] private float speed = 3;

        // protected static으로 외부 접근을 차단하고, 읽기 전용 프로퍼티 제공
        protected static bool isTransitioning = false;

        // 읽기 전용 프로퍼티 (외부에서 직접 값을 변경하지 못하게 함)
        public static bool IsTransitioning
        {
            get { return isTransitioning; }
        }

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual IEnumerator In()
        {
            isTransitioning = true;
            gameObject.SetActive(true);
            canvasGroup.alpha = 0;

            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, Time.deltaTime * speed);

                if (Mathf.Abs(canvasGroup.alpha - 1f) < 0.01f)
                {
                    canvasGroup.alpha = 1;
                }

                yield return null;
            }
            isTransitioning = false;
        }

        public virtual IEnumerator Out()
        {
            isTransitioning = true;
            canvasGroup.alpha = 1;

            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * speed);
                if (Mathf.Abs(canvasGroup.alpha) < 0.01f) canvasGroup.alpha = 0;

                yield return null;
            }

            gameObject.SetActive(false);
            isTransitioning = false;
        }
    }
}