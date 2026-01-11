using UnityEngine;

namespace Eclipse.Game
{
    public interface IDamageable
    {
        void TakeDamage(string causerTag, float damage, bool isCritical = false, float knockbackForce = 0);
    }
}