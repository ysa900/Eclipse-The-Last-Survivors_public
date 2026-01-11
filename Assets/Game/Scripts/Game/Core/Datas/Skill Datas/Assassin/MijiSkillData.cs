using UnityEngine;

namespace Eclipse.Game
{
    [CreateAssetMenu(fileName = "MijiSkillData", menuName = "Scriptable Object/MijiSkillData")]
    public class MijiSkillData : ScriptableObject
    {
        [Header("# Main Info")]
        public string[] skillName;
        public string[] skillDescription;
    
        [Header("# Data")]
        public int[] level;
        public float[] levelCoefficient;
        public float[] Delay; // ÄðÅ¸ÀÓ
        public float[] aliveTime;
    
        [Header("# Skill")]
        public bool[] skillSelected;
        public Sprite[] skillicon;
    }
}