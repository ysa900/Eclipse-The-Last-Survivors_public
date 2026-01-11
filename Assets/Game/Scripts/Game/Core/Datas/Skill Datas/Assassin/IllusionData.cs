using UnityEngine;

namespace Eclipse.Game
{
    [CreateAssetMenu(fileName = "IllusionData", menuName = "Scriptable Object/IllusionData")]
    
    public class IllusionData : ScriptableObject
    {
        // 환영 정보
        [Header("# 환영 정보")]
        public float speed;
        public float hp;
        public float maxHp;
    }
}