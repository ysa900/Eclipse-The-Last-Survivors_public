namespace Eclipse.Game
{
    public class BackGroundSkill : Skill, IPoolingObject
    {
        public bool isStaySkill; // 몇초동안 지속되다가 사라지는 스킬이냐
    
        public float scale;
    }
}