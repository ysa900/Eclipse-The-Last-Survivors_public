using UnityEngine;
using UnityEngine.Tilemaps;

namespace Eclipse.Game
{
    public class ObstacleVisble : MonoBehaviour
    {
        private Transform playerTransform;

        [SerializeField] private float alphaSpeed;
        private float thickness = -0.5f;
    
        SpriteRenderer spriteRenderer;
        Tilemap tile;
        Color srAlphaColor, tileAlphaColor;
        float playerYpostion;

        ObstacleVisibleTrigger trigger;
        public bool isTriggerMatch = false;
        bool isTwowayObstacle = false;

        GameObject upperClearwall, underClearwall;
    
        private void Start()
        {
            trigger = GetComponentInChildren<ObstacleVisibleTrigger>();
            switch (name)
            {
                case "opaque_wall":
                    tile = GetComponent<Tilemap>();
                    tileAlphaColor = tile.color;
                    break;
                default:
                    spriteRenderer = GetComponent<SpriteRenderer>();
                    srAlphaColor = spriteRenderer.color;
                    break;
            }
    
            switch (name)
            {
                case "Left Pillar":
                case "Right Pillar":
                case var nm when nm.StartsWith("Tree"):
                case "Pillar":
                case "StatueObstacle1(Clone)":
                case "Statue":
                case "The Piper(Clone)":
                case "The Angle(Clone)":
                    isTwowayObstacle = true;
                    upperClearwall = transform.Find("Upper Clear wall").gameObject;
                    underClearwall = transform.Find("Under Clear wall").gameObject;
                    break;
            }

            playerTransform = PlayerManager.player.transform;
        }
        private void Update()
        {
            if (isTwowayObstacle)
            {
                playerYpostion = playerTransform.position.y;

                bool playerIsAbove = playerYpostion >= transform.position.y + thickness;
                bool playerIsBelowOutOfRange = !playerIsAbove && !IsPlayerInRange();

                if (playerIsAbove || playerIsBelowOutOfRange)
                {
                    underClearwall.SetActive(true);
                    upperClearwall.SetActive(false);
                    spriteRenderer.sortingOrder = 7;
                }
                else // 아래 + 범위 안
                {
                    underClearwall.SetActive(false);
                    upperClearwall.SetActive(true);
                    spriteRenderer.sortingOrder = 3;
                }
            }

            float targetAlpha = isTriggerMatch || IsPlayerInRange() ? 0.5f : 1f;
            float lerpSpeed = Time.deltaTime * alphaSpeed;

            if (spriteRenderer == null)
            {
                tileAlphaColor.a = Mathf.Lerp(tileAlphaColor.a, targetAlpha, lerpSpeed);
                tile.color = tileAlphaColor;
            }
            else
            {
                srAlphaColor.a = Mathf.Lerp(srAlphaColor.a, targetAlpha, lerpSpeed);
                spriteRenderer.color = srAlphaColor;
            }
        }

        private bool IsPlayerInRange()
        {
            float scaleSize = 5f / 2f;
            Vector2 offset = new Vector2(-0.1f / scaleSize, -0.75f / scaleSize);
            Vector2 myPostion = (Vector2)transform.position + offset;
            float sqrDistance = (myPostion - (Vector2)playerTransform.position).sqrMagnitude;
            float radius = 2.2f / scaleSize;

            return sqrDistance < radius * radius;
        }
        private void OnDrawGizmosSelected()
        {
            if (playerTransform == null) return;

            float scaleSize = 5f / 2f;
            Vector2 offset = new Vector2(0f / scaleSize, -0.75f / scaleSize);
            Vector2 myPosition = (Vector2)transform.position + offset;
            float radius = 2.2f / scaleSize;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(myPosition, radius);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
            {
                return;
            }
            
            if (trigger == null)
            {
                isTriggerMatch = true;
            }
        }
    
        public void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
            {
                return;
            }

            if (trigger == null)
            {
                isTriggerMatch = false;
            }
        }
    }
}