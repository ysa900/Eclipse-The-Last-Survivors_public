using UnityEngine;

namespace Eclipse.Game
{
    public class RandomTrackingSkill : Skill, IPoolingObject
    {
        Rigidbody2D rigid; // 물리 입력을 받기위한 변수

        public Vector2 virtualPosition;
        public Vector2 myPosition;
        public Vector2 direction;

        public float scale;
        public bool isFlipped;
        private float virtualR;
        public float angle;

        protected override void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();

            base.Awake();
        }

        public override void Init()
        {
            base.Init();

            myPosition = PlayerManager.player.transform.position;

            // 플레이어 위치에서 시작
            X = myPosition.x;
            Y = myPosition.y;
        }

        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;

            if (destroySkill)
            {
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

                PoolManager.instance.ReturnSkill(this, returnIndex);
                return;
            }
            else
            {
                MoveToRandomPosition();
            }

            base.Update();
        }

        public void SetRandomPosition()
        {
            angle = UnityEngine.Random.Range(0f, 360f);

            virtualPosition = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            direction = virtualPosition.normalized;
        }

        protected void MoveToRandomPosition()
        {
            rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);

            X = transform.position.x;
            Y = transform.position.y;
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