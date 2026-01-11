using UnityEngine;

namespace Eclipse.Game
{
    public class Boss_AttackWave : BossSkill
    {
        private bool isLeftSide;

        private float startSpeed = 1; // 잠깐 천천히가다가 가속 
        private float maxSpeed = 20; // 가속 완료 속도
        private float speed; // 진짜 속도
        private float slowTime = 0.15f; // 천천히 가는 시간
        private float slowTimer = 0;
        private float accelerationTime = 0.3f; // 가속 시간
        private float t = 0f; // 경과 시간

        Rigidbody2D rigid; // 물리 입력을 받기위한 변수

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();

            aliveTime = 2f;
        }

        public override void Init()
        {
            if (!(boss == null))
            {
                // 가속 관련 변수 초기화
                speed = startSpeed;
                t = 0f; 
                slowTimer = 0f;

                aliveTimer = 0f;
                isLeftSide = boss.isBossLookLeft;
                transform.position = boss.transform.position;
            }
        }

        private void FixedUpdate()
        {
            int signX = isLeftSide ? -1 : 1; 
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * signX; // x 값을 부호에 따라 설정
            transform.localScale = scale; // 변경된 scale 값을 다시 할당

            Vector2 direction = isLeftSide ? Vector2.left : Vector2.right;

            rigid.MovePosition(rigid.position + direction.normalized * speed * Time.fixedDeltaTime);

            X = transform.position.x;
            Y = transform.position.y;

            if (aliveTimer > aliveTime)
            {
                PoolManager.instance.ReturnBossSkill(this, index);
            }

            if (slowTimer >= slowTime)
            {
                AccelerateSpeed();
            }
            else
            {
                slowTimer += Time.fixedDeltaTime;
            }
            aliveTimer += Time.fixedDeltaTime;
        }

        private void AccelerateSpeed()
        {
            float k = 5f; // 가속 곡선 조절 (값이 클수록 더 빠르게 증가)

            t += Time.fixedDeltaTime; // 경과 시간 증가
            speed = maxSpeed * (1 - Mathf.Exp(-k * t / accelerationTime)); // 지수 함수 적용
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IPlayer iPlayer = collision.GetComponent<IPlayer>();

            if (iPlayer == null)
            {
                return;
            }

            iPlayer.TakeDamageOneTime(damage);
        }
    }
}