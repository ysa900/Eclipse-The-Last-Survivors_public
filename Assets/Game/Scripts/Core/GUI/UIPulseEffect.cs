using UnityEngine;

namespace Eclipse
{

    public class UIPulseEffect : MonoBehaviour
    {
        public float pulseSpeed = 2f;         // 속도 (주파수)
        public float scaleAmount = 0.1f;      // 얼마나 커질지
        private Vector3 originalScale;
        private float timer;

        public bool useUnscaledTime = true;

        void Start()
        {
            originalScale = transform.localScale;
        }

        void Update()
        {
            float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            timer += delta;

            float scaleOffset = Mathf.Sin(timer * pulseSpeed) * scaleAmount;
            transform.localScale = originalScale + Vector3.one * scaleOffset;
        }
    }
}