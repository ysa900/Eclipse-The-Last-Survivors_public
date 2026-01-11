using System;
using UnityEngine;

namespace Eclipse.Game
{
    public class BossRoom : MonoBehaviour
    {
        // �÷��̾ Ʈ���ŵǸ� TilemapManager���� �˷��ִ� �뵵
        public Action onPlayerTriggerEntered;
        public Action onPlayerTriggerExited;
    
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
            {
                return;
            }
            onPlayerTriggerEntered();
        }
    
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
            {
                return;
            }

            onPlayerTriggerExited();
        }
    }
}