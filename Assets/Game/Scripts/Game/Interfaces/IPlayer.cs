namespace Eclipse.Game
{
    public interface IPlayer
    {
        void GetExp(int expAmount);
        public void RestoreHP(float restoreAmount);
        void TakeDamageOneTime(float damage);
        void TakeDamageConstantly(bool isStartTakingDamage, float damage = 0);
        void GetCoin(int coinAmount);
    }
}