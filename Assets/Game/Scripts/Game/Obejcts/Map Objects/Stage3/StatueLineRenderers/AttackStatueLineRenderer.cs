using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class AttackStatueLineRenderer : StatueLineRenderer
    {
        private CapsuleCollider2D lineCollider;  // 라인 렌더러를 위한 CapsuleCollider

        private float damage = 2f; // 데미지
        private bool isFading;

        protected override void Awake()
        {
            base.Awake();
            statue = GetComponentInParent<AttackStatue>();

            // 새로운 CapsuleCollider2D 추가 및 설정
            lineCollider = gameObject.AddComponent<CapsuleCollider2D>();
            lineCollider.isTrigger = true; // Trigger로 설정하여 충돌 감지
            lineCollider.direction = CapsuleDirection2D.Horizontal;
            isFading = false;
        }

        protected override void Start()
        {
            base.Start();

            lineRenderer.startWidth = 0.25f; // 선의 시작 두께
            lineRenderer.endWidth = 0.25f;   // 선의 끝 두께
            lineRenderer.material = lineSprites[1];
            lineRenderer.startColor = Color.red; // 선의 시작 색상
            lineRenderer.endColor = Color.red;   // 선의 끝 색상
        }

        protected override void Update()
        {
            if (isFading) return;

            base.Update();

            // LineRenderer의 시작점과 끝점 설정
            lineRenderer.SetPosition(0, lineStartPosition);
            lineRenderer.SetPosition(1, lineEndPosition);

            AdjustLineRendererColliderSize();
        }

        private void AdjustLineRendererColliderSize()
        {
            // CapsuleCollider2D 크기 및 위치 설정
            Vector2 midpoint = (lineStartPosition + lineEndPosition) / 2; // 중간 지점
            float distance = Vector2.Distance(lineStartPosition, lineEndPosition); // 거리 계산

            float statueSize = statue.transform.localScale.y;

            // 라인렌더러 콜라이더 사이즈 조절
            lineCollider.size = new Vector2(distance / statueSize, lineRenderer.startWidth / statueSize); // 길이를 distance로, 너비는 선의 두께로 설정
            lineCollider.transform.position = midpoint; // 중간 지점에 위치
            lineCollider.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg); // 각도를 회전하여 선에 맞춤
        }

        public IEnumerator FadeLineAlpha(float startAlpha, float endAlpha, float duration)
        {
            isFading = true;

            float elapsed = 0f;

            Color startColor = lineRenderer.startColor;
            Color endColor = lineRenderer.endColor;

            while (elapsed < duration)
            {
                if (gameObject == null || lineRenderer == null)
                {
                    yield break; // 게임 오브젝트나 LineRenderer가 없으면 코루틴 종료
                }
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                Color newStartColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
                Color newEndColor = new Color(endColor.r, endColor.g, endColor.b, alpha);

                lineRenderer.startColor = newStartColor;
                lineRenderer.endColor = newEndColor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // 마지막 보정
            lineRenderer.startColor = new Color(startColor.r, startColor.g, startColor.b, endAlpha);
            lineRenderer.endColor = new Color(endColor.r, endColor.g, endColor.b, endAlpha);

            isFading = false;

            // 알파 값에 따라 콜라이더 활성화/비활성화
            lineCollider.enabled = startAlpha > endAlpha ? false : true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IPlayer iPlayer = other.GetComponent<IPlayer>();

            if (iPlayer == null)
            {
                return;
            }

            iPlayer.TakeDamageConstantly(true, damage);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            IPlayer iPlayer = other.GetComponent<IPlayer>();

            if (iPlayer == null)
            {
                return;
            }

            iPlayer.TakeDamageConstantly(false);
        }
    }
}
