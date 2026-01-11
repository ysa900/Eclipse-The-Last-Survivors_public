using UnityEngine;

namespace Eclipse.Game
{
    public class Shuriken : CameraBoundingSkill
    {
        private void Start()
        {
            speed = 4;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            IDamageable damageableObject = collision.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                float levelProportionalDamage = damage * PlayerManager.player.playerData.level;
                var damageResult = DamageHandler.CalculateDamage(attackPower, levelProportionalDamage, criticalChance, criticalMultiplier);

                damageableObject.TakeDamage(tag, damageResult.FinalDamage, damageResult.IsCritical, knockbackForce);
                onSkillAttack(skillIndex, damageResult.FinalDamage);

                return;
            }

            IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();
            if (damageableSkill != null)
            {
                float levelProportionalDamage = damage * PlayerManager.player.playerData.level;
                var damageResult = DamageHandler.CalculateDamage(attackPower, levelProportionalDamage, criticalChance, criticalMultiplier);

                damageableSkill.TakeDamage(tag, damageResult.FinalDamage, damageResult.IsCritical);
                onSkillAttack(skillIndex, damageResult.FinalDamage);

                return;
            }
        }
    }
}