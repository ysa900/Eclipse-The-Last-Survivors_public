using UnityEngine;

namespace Eclipse
{
    [CreateAssetMenu(fileName = "AssassinData", menuName = "Scriptable Object/AssassinData")]
    
    public class AssassinData : PlayerData
    {
        // 플레이어 정보
        [Header("# 암살자 정보")]
        public float dodgeRate; // 회피율
        public float criticalDamageCoefficient; // 크뎀
    }    
}