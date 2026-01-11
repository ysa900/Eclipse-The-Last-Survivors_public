using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class SmokeScreen : Skill
    {
        // ======================================================
        // delegate 모음

        // 회피율 증가 기록을 위한 delegate
        public delegate void OnDodgeOn(float dodgeRate);
        public OnDodgeOn onDodgeOn;

        // 회피율 증가 기록을 위한 delegate
        public delegate void OnDodgeOff(float dodgeRate);
        public OnDodgeOff onDodgeOff;

        // ======================================================

        public float dodgeRate; // 증가 시킬 회피율

        private bool isDodgeRateNormalized = false; // 회피율 정상화 여부
        private bool isPlayerInsideSmoke = false; // 플레이어가 연막 안에 있는지 여부

        public new void Init()
        {
            aliveTimer = 0;

            isDodgeRateNormalized = false;
            isPlayerInsideSmoke = false;
        }

        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;

            if (destroySkill && !isDodgeRateNormalized)
            {
                if (isPlayerInsideSmoke) // 플레이어가 연막 안에 있을 때만 회피율 감소
                {
                    onDodgeOff?.Invoke(dodgeRate);
                    isPlayerInsideSmoke = false; // 상태를 해제
                }

                isDodgeRateNormalized = true; // 한 번만 정상화
                StartCoroutine(Disappear());

                return;
            }

            base.Update();
        }

        private IEnumerator Disappear()
        {
            animator.SetTrigger("Finish");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FlamesSphere_End"));

            StartCoroutine(Return());
        }

        private IEnumerator Return()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
            PoolManager.instance.ReturnSkill(this, skillIndex);
        }

        public void SetSkillInformation(float x, float y)
        {
            X = x;
            Y = y;
        }

        // 데미지 안들어가게 하고 어쌔신 회피력 조절하기 위함
        protected override void OnTriggerEnter2D(Collider2D collision)
        {

        }

        protected void OnTriggerStay2D(Collider2D collision)
        {
            // 이미 연막 안에 없을 때만
            if (collision.gameObject.tag == "Player" && !isPlayerInsideSmoke)
            {
                onDodgeOn?.Invoke(dodgeRate);
                isPlayerInsideSmoke = true; // 상태를 설정
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player" && isPlayerInsideSmoke) // 연막 안에 있을 때만
            {
                onDodgeOff?.Invoke(dodgeRate);
                isPlayerInsideSmoke = false; // 상태를 해제
            }
        }
    }
}