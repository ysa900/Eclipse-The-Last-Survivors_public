using System;
using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class PurificationStatueLineRenderer : StatueLineRenderer
    {
        public bool isInitialized = false; // 초기화 완료 플래그

        // 점멸 속도를 조절할 변수
        public float blinkSpeed = 2f; // 점멸 속도 (값이 클수록 빠르게 점멸)

        // 기믹 실패시 처리
        public Func<float> getGimmickTimer;
        public float failureEffectTime;
        private bool isTransitionAlreadyStart = false;

        protected override void Awake()
        {
            base.Awake();
            statue = GetComponentInParent<PurificationStatue>();
        }

        protected override void Start()
        {
            base.Start();

            lineRenderer.startWidth = 0.3f; // 선의 시작 두께
            lineRenderer.endWidth = 0.3f;   // 선의 끝 두께
            lineRenderer.material = lineSprites[1];
            lineRenderer.startColor = Color.black; // 선의 시작 색상
            lineRenderer.endColor = Color.black;   // 선의 끝 색상
        }

        protected override void Update()
        {
            if (!isInitialized) return; // 초기화가 완료되지 않았으면 작업하지 않음
            base.Update();

            // 현재 석상에서 타겟 석상으로 레이저 발사
            lineRenderer.SetPosition(0, lineStartPosition); // 석상의 위치
            lineRenderer.SetPosition(1, lineEndPosition);   // 타겟의 위치

            if (getGimmickTimer.Invoke() <= 0) // 기믹 실패 시
            {
                if(!isTransitionAlreadyStart) StartCoroutine(TransitionLineColor(Color.white, new Color32(178, 34, 34, 255), failureEffectTime));
                return;
            }

            SetLineColorByCondition();
            SetLinRendererBlinckingAlpha();
        }

        IEnumerator TransitionLineColor(Color startColor, Color endColor, float duration)
        {
            float elapsedTime = 0f;

            // 초기 색상 및 두께 설정
            lineRenderer.material = lineSprites[3];
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = startColor;
            lineRenderer.startWidth = 0.3f; // 시작 두께
            lineRenderer.endWidth = 0.3f;   // 시작 두께

            while (elapsedTime < duration)
            {
                // 진행 비율 계산
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration); // 진행 비율 (0 ~ 1)

                // 색상 보간
                Color currentColor = Color.Lerp(startColor, endColor, t);

                // 두께 보간
                float currentWidth = Mathf.Lerp(0.3f, 1f, t);

                // LineRenderer에 색상 및 두께 적용
                lineRenderer.startColor = currentColor;
                lineRenderer.endColor = currentColor;
                lineRenderer.startWidth = currentWidth;
                lineRenderer.endWidth = currentWidth;

                yield return null; // 다음 프레임까지 대기
            }

            // 최종 색상 및 두께 적용
            lineRenderer.startColor = endColor;
            lineRenderer.endColor = endColor;
            lineRenderer.startWidth = 1f; // 끝 두께
            lineRenderer.endWidth = 1f;   // 끝 두께
        }


        // 석상이 옳바른 곳에 놓였다는 힌트를 주기 위함
        private void SetLineColorByCondition()
        {
            PurificationStatue purificationStatue = statue as PurificationStatue;
            if (purificationStatue.isStatueInInvertedTriangleGroup && purificationStatue.isStatuePlacedInside)
            {
                lineRenderer.material = lineSprites[3];
                lineRenderer.startColor = Color.white; // 선의 시작 색상
                lineRenderer.endColor = Color.white;   // 선의 끝 색상
            }
            else
            {
                lineRenderer.material = lineSprites[1];
                lineRenderer.startColor = Color.black; // 선의 시작 색상
                lineRenderer.endColor = Color.black;   // 선의 끝 색상
            }
        }

        private void SetLinRendererBlinckingAlpha()
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed)); // 사인 함수를 이용해 0 ~ 1까지 반복적으로 변함을 표현함

            // 현재 색상 가져오기
            Color startColor = lineRenderer.startColor;
            Color endColor = lineRenderer.endColor;

            // 알파 값 설정
            startColor.a = alpha;
            endColor.a = alpha;

            // 변경된 색상 적용
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
        }
    }
}
