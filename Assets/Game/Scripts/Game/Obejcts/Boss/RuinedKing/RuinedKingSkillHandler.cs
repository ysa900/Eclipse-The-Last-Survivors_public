using System;
using UnityEngine;

namespace Eclipse.Game
{
    public class RuinedKingSkillHandler : MonoBehaviour
    {
        // ======================================================
        RuinedKing boss;

        // ======================================================
        // 히트박스, 스킬 관련 변수들
        AttackHitBox[] bossHitBoxes = new AttackHitBox[3];

        float[] hitBoxAliveTime = 
        { 
            3 * AnimationConstants.FrameTime,
            3 * AnimationConstants.FrameTime, 
            4 * AnimationConstants.FrameTime
        };
        float[] hitBoxDamage = { 5f, 5f, 7f };

        // 스킬들 데미지
        float[] bossSkills_Damage = { 7.5f };
        public float nightmareDamageCoefficient; // nightmare 난이도 데미지 계수

        // ======================================================
        // 사용할 액션들
        public Action<int, int, Vector2> onSpawnGimmicEnemies;

        // ======================================================

        private void Awake()
        {
            boss = GetComponent<RuinedKing>();
            bossHitBoxes = GetComponentsInChildren<AttackHitBox>();
        }

        void Start()
        {
            InitializeBossSkillActions();
            InitialzeHitBoxes();
        }

        private void InitializeBossSkillActions()
        {
            boss.onLightAttack1 = OnBossTryLightAttack1;
            boss.onLightAttack2 = OnBossTryLightAttack2;
            boss.onHeavyAttack = OnBossTryHeavyAttack;
            boss.onJumpAttack = OnBossTryJumpAttack;
            boss.onBossChangeDamages = OnBossChangeDamages;
        }
        private void InitialzeHitBoxes()
        {
            // 보스 히트박스들 초기화
            for (int i = 0; i < bossHitBoxes.Length; i++)
            {
                AttackHitBox hitBox = bossHitBoxes[i];

                hitBox.SetAliveTime(hitBoxAliveTime[i]);
                hitBox.damage = hitBoxDamage[i] * nightmareDamageCoefficient;
                hitBox.boss = boss;
                hitBox.DisableHitbox();
            }
        }

        void OnBossTryLightAttack1()
        {
            bossHitBoxes[0].Init();
        }
        void OnBossTryLightAttack2()
        {
            bossHitBoxes[1].Init();
        }
        void OnBossTryHeavyAttack()
        {
            bossHitBoxes[2].Init();
        }

        void OnBossTryJumpAttack()
        {
            JumpAtk_ShockWave jumpAtk_ShockWave = PoolManager.instance.GetBossSkill(0, boss) as JumpAtk_ShockWave;
            jumpAtk_ShockWave.damage = bossSkills_Damage[0] * nightmareDamageCoefficient;
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
