using UnityEngine;

namespace Eclipse.Game
{
    public class Stage3MapBoundary : MonoBehaviour
    {
        Stage3Map stage;
        float upper_mapRadius;
        float lower_mapRadius;
        Vector2 mapCenter;

        private void Start()
        {
            stage = transform.GetComponentInParent<Stage3Map>();
            float stageScale = stage.transform.localScale.x;
            CircleCollider2D mapCircleCollider = stage.GetComponent<CircleCollider2D>();
            upper_mapRadius = 22.5f * stageScale;
            lower_mapRadius = mapCircleCollider.radius * stage.transform.localScale.x;
            mapCenter = (Vector2)stage.transform.position / stage.transform.localScale.x + mapCircleCollider.offset;
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Vector2 enemyPosition = collision.transform.position;
                Vector2 directionToCenter = mapCenter - enemyPosition;
                float distanceToCenter = directionToCenter.magnitude;
                float radius = enemyPosition.y > mapCenter.y ? upper_mapRadius : lower_mapRadius;
                
                if (distanceToCenter >= radius)
                {
                    Vector2 newPosition = mapCenter - directionToCenter.normalized * (radius - 0.5f); // 0.5f 만큼 안쪽으로 이동
                    collision.transform.position = newPosition;
                }
            }
        }
    }
}