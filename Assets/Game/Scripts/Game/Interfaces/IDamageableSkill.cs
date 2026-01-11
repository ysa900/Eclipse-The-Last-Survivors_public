using UnityEngine;

namespace Eclipse.Game
{
    public interface IDamageableSkill
    {
        void TakeDamage(string causerTag, float damage, bool isCritical = false);
    }
}