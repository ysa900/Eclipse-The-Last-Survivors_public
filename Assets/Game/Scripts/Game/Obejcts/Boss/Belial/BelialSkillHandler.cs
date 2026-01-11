using UnityEngine;

namespace Eclipse.Game
{
    public class BelialSkillHandler : MonoBehaviour
    {
        Belial boss;

        Boss_AttackWave bossAttackWave;
        Boss_Laser bossLaser;
        Boss_Grid_Laser bossGridLaser;
        Boss_Genesis bossGenesis;

        // 벨리엘 스킬들 데미지
        protected float[] bossSkills_Damage = { 5f, 1.5f, 1.75f, 2f };
        public float nightmareDamageCoefficient; // nightmare 난이도 데미지 계수

        private void Awake()
        {
            boss = GetComponent<Belial>();
        }

        void Start()
        {
            BelialInit();
        }

        private void BelialInit()
        {
            boss.onBossTryBasicAttack = OnBossTryBasicAttack;
            boss.onBossTryLaserAttack = OnBossTryLaserAttack;
            boss.onBossTryGenesisAttack = OnBossTryGenesisAttack;
            boss.onBossTryGridLaserAttack = OnBossTryGridLaserAttack;
            boss.onBossChangeDamages = OnBossChangeDamages;
        }

        private void OnBossTryBasicAttack()
        {
            bossAttackWave = PoolManager.instance.GetBossSkill(0, boss) as Boss_AttackWave;
            bossAttackWave.damage = bossSkills_Damage[0] * nightmareDamageCoefficient;
        }

        private void OnBossTryLaserAttack(float num)
        {
            bossLaser = PoolManager.instance.GetBossSkill(1, boss, num) as Boss_Laser;

            bossLaser.damage = bossSkills_Damage[1] * nightmareDamageCoefficient;
            bossLaser.laserTurnNum = num;
        }

        private void OnBossTryGridLaserAttack(float x, float y, bool isRightTop)
        {
            bossGridLaser = PoolManager.instance.GetBossSkill(2, boss, x, y, isRightTop) as Boss_Grid_Laser;

            bossGridLaser.damage = bossSkills_Damage[2] * nightmareDamageCoefficient;
            bossGridLaser.X = x;
            bossGridLaser.Y = y;
            bossGridLaser.isLeftTop = isRightTop;
        }

        private void OnBossTryGenesisAttack()
        {
            int num = 30;
            for (int i = 0; i < num; i++)
            {
                bossGenesis = PoolManager.instance.GetBossSkill(3, boss) as Boss_Genesis;

                bossGenesis.damage = bossSkills_Damage[3] * nightmareDamageCoefficient;
                float tmpX = boss.transform.position.x;
                float tmpY = boss.transform.position.y;

                float ranNum = UnityEngine.Random.Range(-4f, 4f);
                float ranNum2 = UnityEngine.Random.Range(-3.2f, 1.6f);

                tmpX += ranNum;
                tmpY += ranNum2;

                bossGenesis.X = tmpX;
                bossGenesis.Y = tmpY;
            }
        }

        void OnBossChangeDamages(float changeCoefficient)
        {
            for (int i = 0; i < bossSkills_Damage.Length; i++)
            {
                bossSkills_Damage[i] *= changeCoefficient;
            }
        }
    }
}