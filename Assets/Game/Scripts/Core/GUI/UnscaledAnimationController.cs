using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse
{
    public class UnscaledAnimationController : MonoBehaviour
    {   
        // targetImage만 Awake로 받아오기, 및 애니메이션 스프라이트는 인스펙터 창에서 끌어다 놓기 
        [Header("Animation Settings")]
        [SerializeField] private Image targetImage; // 애니메이션 대상이 되는 UI의 Image 컴포넌트
        [SerializeField] private Sprite[] animationFrames; // 애니메이션에 사용할 '스프라이트 배열'
        private float frameRate = 4f; // 초당 프레임 수 (FPS)

        private int currentFrame = 0; // 현재 재생 중인 프레임 인덱스
        private bool isAnimating = false; // 애니메이션 활성화 여부

        private void Awake()
        {
            targetImage = this.GetComponent<Image>();
        }

        void Start()
        {
            currentFrame = 0;
        }

        public void StartAnimation()
        {
            if (isAnimating) return; // 이미 애니메이션이 실행 중이면 무시
            
            isAnimating = true;
            StartCoroutine(AnimateFrames());
        }

        public void StopAnimation()
        {
            if (!isAnimating) return; // 애니메이션이 실행 중이지 않으면 무시
            isAnimating = false;
            StopAllCoroutines();
        }

        // 애니메이션 프레임을 순서대로 재생
        private IEnumerator AnimateFrames()
        {
            while (isAnimating)
            {
                // 현재 프레임을 Image의 Source Image로 설정
                targetImage.sprite = animationFrames[currentFrame];

                // 다음 프레임으로 이동
                currentFrame = (currentFrame + 1) % animationFrames.Length;

                // Unscaled Time 기반["Realtime"]으로 대기
                yield return new WaitForSecondsRealtime(1f / frameRate);
            }
        }
    }

}
