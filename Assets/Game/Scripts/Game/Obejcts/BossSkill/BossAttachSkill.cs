using UnityEngine;

namespace Eclipse.Game
{
    public class BossAttachSkill : BossSkill
    {
        // 보스를 기준으로 어디 위치로 생성해야할 지를 받는 변수
        public float xOffset;
        public float yOffset;

        public bool isFlipped; // 스킬 프리팹이 뒤집힌 상태냐
        public bool shouldNotBeFlipped; // 뒤집지 않아야 하는 스킬이냐

        private SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }


        public override void Init()
        {
            base.Init();

            AttachBoss();
        }

        void AttachBoss()
        {
            Vector2 bossPosition = boss.transform.position;

            Y = bossPosition.y + yOffset;
            if (shouldNotBeFlipped)
            {
                X = bossPosition.x + xOffset;
                return;
            }
            X = bossPosition.x + (boss.isBossLookLeft ? -xOffset : xOffset);

            if (isFlipped)
            {
                spriteRenderer.flipX = !boss.isBossLookLeft;
            }
            else
            {
                spriteRenderer.flipX = boss.isBossLookLeft;
            }
        }
    }

}
