namespace Eclipse.Game
{
    public class MeleeMinion : Minion
    {
        protected override void FixedUpdate()
        {
            LookAtTarget();
            MoveToTarget();
        
            base.FixedUpdate();
        }
    }
}