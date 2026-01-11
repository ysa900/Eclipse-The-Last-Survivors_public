using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class PhantomShuriken : EnemyTrackingSkill
    {
        Transform playerTransform;

        private float moveTime = 4f * AnimationConstants.FrameTime;
        private float moveTimer = 0f;

        bool isCorrutineNow = false;

        private void Start()
        {
            playerTransform = PlayerManager.player.transform;
        }

        public override void Init()
        {
            moveTimer = 0;
            isCorrutineNow = false;

            base.Init();
        }

        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime && !isCorrutineNow;

            if (destroySkill)
            {
                StartCoroutine(Disappear());

                return;
            }
            else if (moveTimer < moveTime)
            {
                MoveToEnemy();
            }

            base.Update();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            moveTimer += Time.deltaTime;
        }

        private IEnumerator Disappear()
        {
            isCorrutineNow = true;
            animator.SetTrigger("Finish");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Phantom Shuriken_Finish"));

            StartCoroutine(ReturnToPlayer());
        }

        private IEnumerator ReturnToPlayer()
        {
            float frameTime = 4f * AnimationConstants.FrameTime; // 프레임 시간
            float elapsedTime = 0f; // 경과 시간

            SetDirecionToPlayer(frameTime - elapsedTime);
            // 애니메이션이 끝날 때까지 기다림
            while (true)
            {
                rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime); // 플레이어 방향으로 위치 변경
                
                elapsedTime += Time.deltaTime;

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.normalizedTime >= 0.99f || elapsedTime >= frameTime)
                    break;

                yield return new WaitForFixedUpdate();
            }

            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

            PoolManager.instance.ReturnSkill(this, returnIndex);
        }

        private void SetDirecionToPlayer(float frameTime)
        {
            Vector2 startPosition = transform.position; // 시작 위치
            Vector2 targetPosition = playerTransform.position; // 플레이어 위치

            float distance = Vector2.Distance(startPosition, targetPosition); // 시작 위치와 플레이어 위치 사이의 거리
            speed = distance / frameTime; // 프레임 시간 동안 이동해야 할 속도

            direction = (targetPosition - startPosition).normalized; // 방향 벡터 계산 
        }

        // AssassinSkillManager에서 호출하여 스킬 초기화 시 속도 설정
        public void InitialzeSpeed()
        {
            Vector2 playerPosition = PlayerManager.player.transform.position;
            Vector2 enemyPosition = GetAdjustedEnemyPosition(enemy);
            float distance = Vector2.Distance(playerPosition, enemyPosition); // 플레이어와 적 사이의 거리
            speed = distance / moveTime; // 프레임 시간 동안 이동해야 할 속도
        }
    }
}