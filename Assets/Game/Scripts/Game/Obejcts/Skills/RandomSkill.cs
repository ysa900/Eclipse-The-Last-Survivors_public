using UnityEngine;

namespace Eclipse.Game
{
    public class RandomSkill : Skill
    {
        public float scale;
    
        protected Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    
        protected override void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
    
            base.Awake();
        }
    }
}