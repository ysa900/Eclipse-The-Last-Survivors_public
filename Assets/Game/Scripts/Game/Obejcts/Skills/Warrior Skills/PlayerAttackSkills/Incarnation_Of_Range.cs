using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Incarnation_Of_Range : PlayerAttachSkill
    {
        bool isCorrutineNow = false;

        int normalizeNum = 1;

        public override void Init()
        {
            isCorrutineNow = false;
            normalizeNum = 1;

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
            else
            {
                CheckAndMoveToRandomPoint();
                AttachPlayer();
            }

            base.Update();
        }

        // ��� ����� �� �� ������ �ٸ� ���� ��ġ�� �̵��ϴ� �Լ�
        void CheckAndMoveToRandomPoint()
        {
            bool isAttackFinishedOnce = animator.GetCurrentAnimatorStateInfo(0).IsName("Blood_Legendary_middle")
                    && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= normalizeNum;

            if (isAttackFinishedOnce)
            {
                xOffset = UnityEngine.Random.Range(-1.2f, 1.2f);
                yOffset = UnityEngine.Random.Range(-1.2f, 1.2f);

                normalizeNum++;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.IncarnationOfRange);
            }
        }

        private IEnumerator Disappear()
        {
            isCorrutineNow = true;
            animator.SetTrigger("Finish");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Blood_Legendary_finish"));

            StartCoroutine(Return());
        }
        private IEnumerator Return()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���

            PoolManager.instance.ReturnSkill(this, returnIndex);
        }
    }
}