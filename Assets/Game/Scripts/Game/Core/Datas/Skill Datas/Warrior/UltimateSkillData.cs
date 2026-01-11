using UnityEngine;

namespace Eclipse.Game
{
    [CreateAssetMenu(fileName = "UltimateSkills", menuName = "Scriptable Object/UltimateSkillData")]
    
    public class UltimateSkillData : ScriptableObject
    {
        [Header("# Main Info")]
        public string[] skillName;
    
        [Header("# Data")]
        public string[] levelIncrementType;
        public string[] levelThreeIncrementValue;
        public string[] levelMaxIncrementValue;
    
        [Header("# Ultimate")]
        public bool[] isUltimated;
    }
}