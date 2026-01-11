using UnityEngine;

namespace Eclipse.Game
{
    [CreateAssetMenu(fileName = "Skills2", menuName = "Scriptable Object/SkillData2")]
    
    public class SkillData2 : ScriptableObject
    {
        [Header("# Main Info")]
        [TextArea] public string[] skillName;
        [TextArea] public string[] skillDescription;
    
        [Header("# Data")]
        public int[] level;
        public float[] damage;
        public float[] delay;
        public float[] scale;
        public int[] skillCount;
        public float[] aliveTime;
        public float[] knockbackForce;
        [TextArea] public string[] levelIncrementType;
        [TextArea] public string[] levelThreeIncrementValue;
        [TextArea] public string[] levelMaxIncrementValue;
    
        [Header("# Skill")]
        public bool[] skillSelected;
        public Sprite[] skillicon;

        [Header("# Skill Side Effect")]
        public float[] sideEffect;
    }
}