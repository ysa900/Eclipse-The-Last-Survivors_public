using UnityEngine;

namespace Eclipse.Game
{
    public class StatueFButtonObject : MonoBehaviour
    {
        public PurificationStatue purificationStatue;
        SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            spriteRenderer.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player" && !purificationStatue.isStatueMoving)
            {
                purificationStatue.isPlayerInRange = true;
                spriteRenderer.enabled = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                purificationStatue.isPlayerInRange = false;
                spriteRenderer.enabled = false;
            }
        }
    }
}