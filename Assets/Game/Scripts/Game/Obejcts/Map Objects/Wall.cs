using UnityEngine;

namespace Eclipse.Game
{
    public class Wall : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Obstacle"))
            {
                Destroy(collision.gameObject);
            }
        }
    }
}