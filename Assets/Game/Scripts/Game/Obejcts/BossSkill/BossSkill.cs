namespace Eclipse.Game
{
    public class BossSkill : Object, IPoolingObject
    {
        protected float aliveTime; // 스킬 생존 시간을 체크할 변수
        protected float aliveTimer; // 스킬 생존 시간 타이머

        public virtual void Init() { } // PoolManager때문에 이거 지우면 안됨
    
        public Boss boss;
        
        public float damage;
    
        public int index;
    }
}