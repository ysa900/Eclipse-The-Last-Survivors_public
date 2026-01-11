using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class Snare : EnemyOnSkill
    {
        public override void Init()
        {
            base.Init();

            Invoke("MakeDebuffFloor", 0.1f);
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

            base.Update();
        }

        protected override void OnAfterDamageApplied(Collider2D collision, DamageResult damageResult)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {   
                var boss = collision.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.compressedDamage += damageResult.FinalDamage;
                }
            }

            // LayerMask가 "Enemy", "Boss"인 경우
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")
                || collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                collision.gameObject.GetComponent<IDebuffable>().MakeBind(aliveTime - aliveTimer);
            }
        }

        // Invoke()로 호출되는 메서드
        private void MakeDebuffFloor()
        {
            // 속박 장판
            Skill debuffFloor = PoolManager.instance.GetSkill(8) as DebuffFloor;

            // 현재 VineTrap의 위치에서 장판 생성
            debuffFloor.X = X;
            debuffFloor.Y = Y - 0.7f;

            debuffFloor.AliveTime = aliveTime + 0.5f;
            debuffFloor.skillIndex = skillIndex;
            debuffFloor.Damage = damage * 0.7f;

            debuffFloor.onSkillAttack = this.onSkillAttack;

            Transform parent = debuffFloor.transform.parent; // 스킬 크기 설정

            debuffFloor.transform.parent = null;
            debuffFloor.transform.localScale = new Vector3(2f, 2f, 0);
            debuffFloor.transform.parent = parent;
        }
    }
}