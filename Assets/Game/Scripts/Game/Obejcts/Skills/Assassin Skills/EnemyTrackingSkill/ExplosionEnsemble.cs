using UnityEngine;

namespace Eclipse.Game
{
    public class ExplosionEnsemble : EnemyTrackingSkill
    {
        public int criticalCoefficient;
    
        protected override void Update()
        {
            bool destroySkill = aliveTimer >= aliveTime;
    
            if (destroySkill)
            {
                // 폭발 이펙트 생성
                Skill explosionSkill = PoolManager.instance.GetSkill(11) as ExplosionEnsemble_Main;
                // 수리검 위치에 이펙트 생성
                explosionSkill.X = X;
                explosionSkill.Y = Y;
    
                explosionSkill.AliveTime = 2f;
                explosionSkill.Damage = damage;
                explosionSkill.skillIndex = skillIndex;
    
                Transform parent = explosionSkill.transform.parent;
    
                explosionSkill.transform.parent = null;
                explosionSkill.transform.localScale = new Vector3(4f, 4f, 0);
                explosionSkill.transform.parent = parent;
    
                // 적 바라보게 보정
                Vector2 direction = new Vector2(enemyPosition.x - X, enemyPosition.y - Y);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                // 프리팹 돌리고 싶다면 angle 수정할 것
                Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
                Quaternion rotation = Quaternion.Slerp(explosionSkill.transform.rotation, angleAxis, 5f);
                explosionSkill.transform.rotation = rotation;
                MakeRightSprite();
    
                PoolManager.instance.ReturnSkill(this, returnIndex);
            }

            MoveToEnemy();

            base.Update();
        }
    }
}