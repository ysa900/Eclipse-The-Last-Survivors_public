using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class BossConstantDamageSkill : BossSkill
    {
        protected float safeTime; // 플레이어가 피할 수 있게 아주 살짝 데미지 안들어가는 시간
        protected float safeTimer; // 데미지 안들어가는 시간 타이머

        protected bool isPlayerEnteredInSafeTime; // 플레이어가 SafeTime에 스킬에 Trigger Enter됐을 때 처리해주기 위함
        protected IPlayer iPlayer;

        protected bool isDisappearCoroutineNow;
        protected bool isDamageCoroutineNow;
        private Coroutine damageCoroutine;
        protected Animator animator;

        public override void Init()
        {
            isDisappearCoroutineNow = false;
            isDamageCoroutineNow = false;
            aliveTimer = 0f;
            safeTimer = 0f;
            animator.SetBool("Stay", false);
        }

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            animator.SetBool("Stay", false);
        }

        protected virtual void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;

            if (destroySkill && !isDisappearCoroutineNow)
            {
                StartCoroutine(Disappear());
                return;
            }

            if (safeTimer >= safeTime)
            {
                animator.SetBool("Stay", true);
            }

            aliveTimer += Time.deltaTime;
            safeTimer += Time.deltaTime;
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            IPlayer iPlayer = collision.GetComponent<IPlayer>();

            if (iPlayer == null)
            {
                return;
            }

            if (safeTimer >= safeTime)
            {
                iPlayer.TakeDamageConstantly(true, damage);
            }
            else if(!isDamageCoroutineNow)
            {
                damageCoroutine = StartCoroutine(TakeDelayedDamage(iPlayer, safeTime - safeTimer));
            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            IPlayer iPlayer = collision.GetComponent<IPlayer>();

            if (iPlayer == null)
            {
                return;
            }

            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                isDamageCoroutineNow = false;
            }

            if (safeTimer >= safeTime)
            {
                iPlayer.TakeDamageConstantly(false);
            }
        }

        private IEnumerator TakeDelayedDamage(IPlayer iPlayer, float delayedTime)
        {
            isDamageCoroutineNow = true;
            yield return new WaitForSeconds(delayedTime);

            iPlayer.TakeDamageConstantly(true, damage);
            isDamageCoroutineNow = false;
        }

        protected virtual IEnumerator Disappear()
        {
            yield return new WaitForSeconds(0);
        }
    }
}
