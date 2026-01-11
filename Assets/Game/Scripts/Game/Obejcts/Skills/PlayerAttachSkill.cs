using UnityEngine;

namespace Eclipse.Game
{
    public class PlayerAttachSkill : Skill, IPoolingObject
    {
        // 플레이어 좌표를 기준으로 위치를 어디로 가야하나를 받는 변수
        public float xOffset;
        public float yOffset;
    
        public bool isAttachSkill; // 플레이어 근처에 붙어다니는 스킬이냐
        public bool isFlipped; // 스킬 프리팹이 뒤집힌 상태냐
        public bool isYFlipped; // 스킬을 y축으로 뒤집어야되냐
        public bool shouldNotBeFlipped; // 뒤집지 않아야 하는 스킬이냐
    
        public override void Init()
        {
            base.Init();
    
            AttachPlayer();
        }
    
        // 플레이어에 붙어다니는 스킬
        public virtual void AttachPlayer()
        {
            Player player = PlayerManager.player;
            Vector3 playerPosition = player.transform.position;

            Y = playerPosition.y + yOffset;
            if (shouldNotBeFlipped)
            {
                X = playerPosition.x + xOffset;
                return;
            }
            X = player.transform.position.x + (player.isPlayerLookLeft ? -xOffset : xOffset);
            
            if (isFlipped)
            {
                spriteRenderer.flipX = !player.isPlayerLookLeft;
            }
            else if (isYFlipped)
            {
                spriteRenderer.flipY = !player.isPlayerLookLeft;
            }
            else
            {
                spriteRenderer.flipX = player.isPlayerLookLeft;
            }
        }
    }
}