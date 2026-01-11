using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class JumpAtk_ShockWave : BossSkill
    {
        float damageTime = 3f * AnimationConstants.FrameTime; // 데미지 타이머
        float damageTimer = 0f; // 데미지 타이머

        Animator animator;
        Collider2D col;
        Vector2 offset = new Vector2(-0.085f, 0.283f); // 스킬의 위치에 더해줄 오프셋

        private void Awake()
        {
            animator = GetComponent<Animator>();
            col = GetComponent<Collider2D>();
        }

        public override void Init()
        {
            SetPosition();
            StartCoroutine(WaitUntilAnimationFinish_AndReturn());
            col.enabled = true;
            damageTimer = 0f; // 데미지 타이머 초기화
        }

        void SetPosition()
        {
            // 보스의 위치에 오프셋을 더하여 스킬의 위치를 설정
            transform.position = (Vector2)boss.transform.position + offset;

            // 스킬이 보스의 방향을 바라보도록 설정
            if (boss.isBossLookLeft)
            {
                Vector3 scale = transform.localScale;
                scale.x = -Mathf.Abs(scale.x); // x 값을 부호에 따라 설정
                transform.localScale = scale; // 변경된 scale 값을 다시 할당
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x); // x 값을 부호에 따라 설정
                transform.localScale = scale; // 변경된 scale 값을 다시 할당
            }
        }

        private void Update()
        {
            if (damageTime <= damageTimer)
            {
                col.enabled = false;
                return;
            }
            damageTimer += Time.deltaTime;
        }

        IEnumerator WaitUntilAnimationFinish_AndReturn()
        {
            // 애니메이션이 끝날 때까지 기다림
            yield return new WaitUntil(() =>
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 레이어 0 사용
                return stateInfo.IsName("JumpAtk_ShockWave") && stateInfo.normalizedTime >= 0.9f;
            });

            PoolManager.instance.ReturnBossSkill(this, index);
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