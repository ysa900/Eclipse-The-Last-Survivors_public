using UnityEngine;

namespace Eclipse.Game
{
    public class DefenceStatueLineRenderer : StatueLineRenderer
    {
        DefenceStatue defenceStatue;

        [SerializeField] private LayerMask layerMask; // 플레이어 레이어 마스크

        protected override void Awake()
        {
            base.Awake();
            statue = GetComponentInParent<DefenceStatue>();
            defenceStatue = statue as DefenceStatue;
        }

        protected override void Start()
        {
            base.Start();

            lineRenderer.startWidth = 0.5f; // 선의 시작 두께
            lineRenderer.endWidth = 0.5f;   // 선의 끝 두께
            lineRenderer.material = lineSprites[3];
            //lineRenderer.startColor = Color.white; // 선의 시작 색상
            //lineRenderer.endColor = Color.black;   // 선의 끝 색상

            layerMask = LayerMask.GetMask("Player");
        }

        protected override void Update()
        {
            base.Update();
            HandleOverlapBoxProcessAndSetInvincibility();
        }

        private void HandleOverlapBoxProcessAndSetInvincibility()
        {
            // 중심점과 방향 계산
            Vector2 direction = (lineEndPosition - lineStartPosition).normalized;
            Vector2 center = (lineStartPosition + lineEndPosition) / 2; // 중심점
            float distance = Vector2.Distance(lineStartPosition, lineEndPosition);

            // OverlapBox의 크기 설정: 너비는 라인렌더러의 두께, 길이는 두 점 사이의 거리
            Vector2 boxSize = new Vector2(distance, lineRenderer.startWidth);

            // OverlapBox로 충돌 감지
            Collider2D hit = Physics2D.OverlapBox(center, boxSize, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, layerMask);

            if (hit != null)
            {
                // 플레이어가 중간에 있다면 끝점을 플레이어 위치로 설정
                if (hit.gameObject == player.gameObject)
                {
                    lineRenderer.SetPosition(0, lineStartPosition); // 석상의 위치
                    lineRenderer.SetPosition(1, player.transform.position); // 플레이어의 위치

                    // 플레이어가 가로막고 있을 시 석상 때릴 수 있게 처리
                    defenceStatue.DeactivateShield(); // 석상 방패 비활성화
                }
                else
                {
                    // 플레이어가 아니면 타겟 위치로 설정
                    lineRenderer.SetPosition(0, lineStartPosition); // 석상의 위치
                    lineRenderer.SetPosition(1, lineEndPosition);   // 타겟의 위치

                    // 플레이어가 가로막고 있지 않을 시 석상은 무적 처리
                    defenceStatue.ActivateShield(); // 석상 방패 활성화
                }
            }
            else
            {
                // 아무도 안맞고 있으면 타겟 위치로 설정
                lineRenderer.SetPosition(0, lineStartPosition); // 석상의 위치
                lineRenderer.SetPosition(1, lineEndPosition);   // 타겟의 위치

                // 플레이어가 가로막고 있지 않을 시 석상은 무적 처리
                defenceStatue.ActivateShield(); // 석상 방패 활성화
            }
        }
    }
}
