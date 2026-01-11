using UnityEngine;

namespace Eclipse.Game
{
    public static class DamageHandler
    {
        public readonly struct DamageResult
        {
            public float FinalDamage { get; }
            public bool IsCritical { get; }

            public DamageResult(float damage, bool critical)
            {
                FinalDamage = damage;
                IsCritical = critical;
            }
        }

        public static DamageResult CalculateDamage(
            float attackPower, // 플레이어 상점 강화 공격력
            float skillDamage, // 스킬 데미지
            float criticalChance, // 크리티컬 확률
            float criticalMultiplier // 크리티컬 계수
        )
        {
            // 기본 데미지 계산
            float finalDamage = skillDamage * (1f + attackPower);
            bool isCritical = false;

            // 도적 비급 패시브 스킬 효과 적용
            if (Miji_SkillManager.instance != null)
            {
                MijiSkillData passiveMijiSkillData = Miji_SkillManager.instance.passiveMijiSkillData;

                if (passiveMijiSkillData.skillSelected[0])
                {
                    criticalMultiplier += passiveMijiSkillData.levelCoefficient[0];
                }
                if (passiveMijiSkillData.skillSelected[2])
                {
                    finalDamage *= 1 + passiveMijiSkillData.levelCoefficient[2];
                }
                if (passiveMijiSkillData.skillSelected[4])
                {
                    criticalChance += passiveMijiSkillData.levelCoefficient[4];
                }
            }

            // 크리티컬 적용
            if (Random.value < criticalChance)
            {
                if (criticalMultiplier <= 0)
                {
                    Debug.Log("Critical multiplier is less than or equal to zero. Setting to default value of 1.");
                    criticalMultiplier = 1f; // 기본값으로 설정
                }
                finalDamage *= criticalMultiplier;
                isCritical = true;
            }

            return new DamageResult(finalDamage, isCritical);
        }
    }
}
