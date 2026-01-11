using UnityEngine;
using UnityEngine.EventSystems;

namespace Eclipse
{
    public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Vector3 originalScale;
        private Vector3 targetScale;
        public float scaleMultiplier = 1.2f;
        public float animationSpeed = 10f;

        protected bool isAnimating = false;
        private float epsilon = 0.001f;

        protected virtual void Start()
        {
            originalScale = transform.localScale;
            targetScale = originalScale;
        }

        void OnEnable()
        {
            // 리셋 스케일 상태
            if (originalScale == Vector3.zero)
            {
                originalScale = transform.localScale; // 안전장치
            }

            transform.localScale = originalScale;
            targetScale = originalScale;
            isAnimating = false;
        }

        protected void Update()
        {
            if (!isAnimating) return;

            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);

            // 충분히 가까워지면 애니메이션을 중지
            if (Vector3.Distance(transform.localScale, targetScale) < epsilon)
            {
                transform.localScale = targetScale;
                isAnimating = false;
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            targetScale = originalScale * scaleMultiplier;
            isAnimating = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = originalScale;
            isAnimating = true;
        }

        public void SetOriginalScale(Vector3 scale)
        {
            originalScale = scale;
        }
    }
}