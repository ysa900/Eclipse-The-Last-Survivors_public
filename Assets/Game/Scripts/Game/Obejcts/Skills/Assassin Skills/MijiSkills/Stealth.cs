using System;
using UnityEngine;

namespace Eclipse.Game
{
    public class Stealth : Object
    {
        //==================================================================
        private bool isStealthActive = false; // 은신 활성화 여부
        public float aliveTimer = 0f; // 은신 지속 시간 카운트
        public float aliveTime = 0f; // 은신 지속 시간

        //==================================================================
        SpriteRenderer _assassinSpriteRenderer; // 어쌔신의 스프라이트 렌더러
        SpriteRenderer assassinSpriteRenderer
        {
            get
            {
                if (_assassinSpriteRenderer == null)
                {
                    _assassinSpriteRenderer = transform.parent?.GetComponentInParent<SpriteRenderer>();
                }
                return _assassinSpriteRenderer;
            }
        }
        CapsuleCollider2D _assassinCollider;
        CapsuleCollider2D assassinCollider
        {
            get
            {
                if (_assassinCollider == null)
                {
                    _assassinCollider = transform.parent?.GetComponentInParent<CapsuleCollider2D>();
                }
                return _assassinCollider;
            }
        }

        AssassinData _playerData;
        AssassinData playerData
        {
            get
            {
                if (_playerData == null)
                {
                    _playerData = PlayerManager.player.playerData as AssassinData;
                }
                return _playerData;
            }
        }

        //==================================================================
        // Action
        public Action<bool> onStealthStateChanged; // 은신 상태인지 알려주는 액션

        //==================================================================

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            aliveTimer = 0f;
        }

        private void Update()
        {
            bool isFin = aliveTimer >= aliveTime;

            if (isFin)
            {
                RestorePlayer();
            }
            else
            {
                AttachPlayer();
            }

            aliveTimer += Time.deltaTime;
        }

        public void AttachPlayer()
        {
            X = PlayerManager.player.transform.position.x;
            Y = PlayerManager.player.transform.position.y;
        }

        public void MakePlayerStealth()
        {
            Color alpha = assassinSpriteRenderer.color;
            alpha.a = 100f / 255f;
            assassinSpriteRenderer.color = alpha;

            isStealthActive = true;
            playerData.dodgeRate += 10f; // 은신 상태에서 적의 공격을 회피할 확률 증가
            onStealthStateChanged?.Invoke(isStealthActive);
        }

        public void RestorePlayer()
        {
            Color alpha = assassinSpriteRenderer.color;
            alpha.a = 1f;
            assassinSpriteRenderer.color = alpha;

            isStealthActive = false;
            playerData.dodgeRate -= 10f; // 은신 상태 끝나면, 적의 공격을 회피할 확률 감소
            onStealthStateChanged?.Invoke(isStealthActive);
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
                other.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                assassinCollider.isTrigger = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
                other.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                assassinCollider.isTrigger = false;
            }
        }
    }
}
