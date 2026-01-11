using UnityEngine;

namespace Eclipse.Game
{
    public class ObstacleVisibleTrigger : MonoBehaviour
    {
        ObstacleVisble ObstacleVisble;

        private void Start()
        {
            ObstacleVisble = GetComponentInParent<ObstacleVisble>();
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
            {
                return;
            }

            ObstacleVisble.isTriggerMatch = true;
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
            {
                return;
            }

            ObstacleVisble.isTriggerMatch = false;
        }
    }
}