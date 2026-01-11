using UnityEngine;

namespace Eclipse.Game
{
    public class HP_Potion : Item
    {
        public float hpAmount;
    
        private void OnTriggerEnter2D(Collider2D collision)
        {
            IPlayer iPlayer = collision.GetComponent<IPlayer>();
    
            if (iPlayer == null)
            {
                return;
            }
    
            iPlayer.RestoreHP(hpAmount);
    
            Destroy(gameObject);
        }
    }
}