using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Eclipse.Game
{
    public class StatueLineRenderer : MonoBehaviour
    {
        public GameObject target; // 타겟
        protected Player player; // 플레이어
        protected Statue statue; // 석상

        // 상속 용 변수들
        protected float angle;
        protected Vector2 lineStartPosition;
        protected Vector2 lineEndPosition;
        protected List<Material> lineSprites = new List<Material>();

        protected CapsuleCollider2D statueCapsuleCollider; // 부모 석상의 CapsuleCollider
        protected LineRenderer lineRenderer;

        protected virtual void Awake()
        {
            statueCapsuleCollider = GetComponentInParent<CapsuleCollider2D>();
            lineSprites.Add(Resources.Load<Material>("Materials/Map Objects/Stage3/Line_Thin Material"));
            lineSprites.Add(Resources.Load<Material>("Materials/Map Objects/Stage3/Line_Middle Material"));
            lineSprites.Add(Resources.Load<Material>("Materials/Map Objects/Stage3/Line_Thick Material"));
            lineSprites.Add(Resources.Load<Material>("Materials/Map Objects/Stage3/Line_Middle_White Material"));
        }

        protected virtual void Start()
        {
            // LineRenderer 컴포넌트 추가
            lineRenderer = gameObject.AddComponent<LineRenderer>();

            // 플레이어 초기화
            player = PlayerManager.player;
        }

        protected virtual void Update()
        {
            CalculateLineRendererPosition();
        }

        private void CalculateLineRendererPosition()
        {
            Vector2[] closetPoints = GetClosestPoints(statueCapsuleCollider, target.GetComponent<CapsuleCollider2D>());
            lineStartPosition = closetPoints[0];
            lineEndPosition = closetPoints[1];
        }

        // 두 개의 CapsuleCollider2D를 받아서 최단 거리의 각도에 해당하는 두 점을 반환하는 함수
        private Vector2[] GetClosestPoints(CapsuleCollider2D sourceCollider, CapsuleCollider2D targetCollider)
        {
            // 각 캡슐 콜라이더의 중심 좌표 계산
            Vector2 sourceCenter = (Vector2)sourceCollider.transform.position + new Vector2(
                sourceCollider.offset.x * sourceCollider.transform.localScale.x,
                sourceCollider.offset.y * sourceCollider.transform.localScale.y
            );

            Vector2 targetCenter = (Vector2)targetCollider.transform.position + new Vector2(
                targetCollider.offset.x * targetCollider.transform.localScale.x,
                targetCollider.offset.y * targetCollider.transform.localScale.y
            );

            // 두 중심 사이의 방향 벡터 계산
            Vector2 direction = (targetCenter - sourceCenter).normalized;

            // 두 중심 사이의 각도 계산
            angle = Mathf.Atan2(direction.y, direction.x);

            return new Vector2[] { sourceCenter, targetCenter }; // 현재는 선이 각 석상의 중심으로 오도록 설정, 아래는 테두리로 오도록 설정

            /*// source 콜라이더의 테두리 점 계산
            Vector2 sourcePoint = GetEllipsePoint(sourceCollider, angle);

            // target 콜라이더의 테두리 점 계산 (반대 방향)
            float reverseAngle = angle + Mathf.PI; // 180도 뒤집기
            Vector2 targetPoint = GetEllipsePoint(targetCollider, reverseAngle);

            return new Vector2[] { sourcePoint, targetPoint };*/
        }

        // 주어진 CapsuleCollider2D와 각도에 따라 타원의 테두리 점을 반환하는 함수
        private Vector2 GetEllipsePoint(CapsuleCollider2D capsuleCollider, float angle)
        {
            // 캡슐 콜라이더의 중심 좌표 계산 (offset에 localScale을 반영)
            Vector2 scaledOffset = new Vector2(
                capsuleCollider.offset.x * capsuleCollider.transform.localScale.x,
                capsuleCollider.offset.y * capsuleCollider.transform.localScale.y
            );
            Vector2 center = (Vector2)capsuleCollider.transform.position + scaledOffset;

            // 캡슐 콜라이더의 가로 및 세로 길이
            float width = capsuleCollider.size.x * capsuleCollider.transform.localScale.x;
            float height = capsuleCollider.size.y * capsuleCollider.transform.localScale.y;

            // 타원의 반지름 (가로 및 세로 반지름)
            float radiusX = width / 2;
            float radiusY = height / 2;

            // 타원의 둘레에서 주어진 각도에 따른 점의 좌표 계산
            float x = center.x + radiusX * Mathf.Cos(angle);
            float y = center.y + radiusY * Mathf.Sin(angle);

            return new Vector2(x, y);
        }
    }
}
