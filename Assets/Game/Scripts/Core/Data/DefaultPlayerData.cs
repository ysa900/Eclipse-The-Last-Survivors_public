using UnityEngine;

namespace Eclipse
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Object/DefaultPlayerData")]

    public class DefaultPlayerData : ScriptableObject
    {
        // 플레이어 정보
        [Header("# 플레이어 디폴트 정보")]
        public float speed;
        public float maxHp;
        public float healthRegen;

        // 플레이어 패시브 효과 관련 변수
        [Header("# 플레이어 디폴트 공통 패시브 효과")]
        public float damageReductionValue; 
        public float magnetRange;          

    }
}