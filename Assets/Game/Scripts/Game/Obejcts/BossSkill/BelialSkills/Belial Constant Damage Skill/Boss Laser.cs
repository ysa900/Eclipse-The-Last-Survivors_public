using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Boss_Laser : BossConstantDamageSkill, IPoolingObject
    {
        float laserHalf = 11.2f / 4; // 레이저 prefab의 절반 길이
        public float laserTurnNum; // 레이저 회전 각도

        public override void Init()
        {
            base.Init();
            AttachBoss();
        }

        protected override void Awake()
        {
            base.Awake();

            aliveTime = 4f;
            safeTime = 1.6f;
        }
    
        // 보스에 붙어다니는 스킬 (현재는 특정 위치에 소환 됨)
        private void AttachBoss()
        {
            Vector2 playerPosition = PlayerManager.player.transform.position;
            Vector2 bossPosition = boss.transform.position;
    
            float compensateNum;
    
            if (boss.isBossLookLeft)
                compensateNum = -2.5f;
            else
                compensateNum = 2.5f;
    
            Vector2 direction = new Vector2(bossPosition.x + compensateNum - playerPosition.x, bossPosition.y - 3f - playerPosition.y + 0.1f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + laserTurnNum;
    
            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
            transform.rotation = rotation;
    
            float zDegree = transform.rotation.eulerAngles.z;
            float tmpX = (float)Mathf.Cos(zDegree * Mathf.Deg2Rad) * laserHalf;
            float tmpY = (float)Mathf.Sin(zDegree * Mathf.Deg2Rad) * laserHalf;
    
            X = boss.X + compensateNum; Y = boss.Y - 3f;
            X -= tmpX; Y -= tmpY;
        }

        protected override IEnumerator Disappear()
        {
            animator.SetTrigger("Exit");

            isDisappearCoroutineNow = true;

            yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기

            PoolManager.instance.ReturnBossSkill(this, index);
        }
    }
}