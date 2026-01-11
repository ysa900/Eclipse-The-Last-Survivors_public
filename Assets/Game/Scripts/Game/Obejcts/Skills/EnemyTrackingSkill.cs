using UnityEngine;

namespace Eclipse.Game
{
    public class EnemyTrackingSkill : Skill, IPoolingObject
    {
        protected bool isBossAppear;
    
        protected Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    
        protected Vector2 enemyPosition;
        public Vector2 myPosition;
        public Vector2 direction;
        public float scale;
        public bool isFlipped;
    
        protected override void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
    
            base.Awake();
        }
    
        public override void Init()
        {
            base.Init();
    
            myPosition = PlayerManager.player.transform.position;
    
            X = myPosition.x;
            Y = myPosition.y;
        }

        public void SetTrackingDirection()
        {
            if (enemy == null) return;

            // 적의 실제 위치 보정
            Vector2 targetPosition = GetAdjustedEnemyPosition(enemy);
            Vector2 myPosition = transform.position;

            // 방향 벡터 계산 (목표 - 현재)
            direction = (targetPosition - myPosition).normalized;
        }

        // 적을 따라가는 스킬
        protected void MoveToEnemy()
        {
            rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime); // Enemy 방향으로 위치 변경
        }
    
        public void MakeRightSprite()
        {
            if (isFlipped)
            {
                spriteRenderer.flipX = !PlayerManager.player.isPlayerLookLeft;
            }
            else
            {
                spriteRenderer.flipX = PlayerManager.player.isPlayerLookLeft;
            }
        }
    }
}