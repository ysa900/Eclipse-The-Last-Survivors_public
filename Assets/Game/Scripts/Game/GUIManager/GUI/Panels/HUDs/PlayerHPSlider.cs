using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.Panels
{
    public class PlayerHPSlider : MonoBehaviour
    {
        private Slider hpSlider;
        private RectTransform sliderRectTransform;
        private Vector3 offset = new Vector3(0, -0.35f, 0); // UI 위치 보정용 오프셋

        private Transform playerTransform;
        private Camera mainCamera; // 메인 카메라

        private PlayerData _playerData;
        public PlayerData PlayerData
        {
            get { return _playerData; }
            set { _playerData = value; }
        }

        private void Awake()
        {
            hpSlider = GetComponent<Slider>();
            sliderRectTransform = GetComponent<RectTransform>();
            mainCamera = Camera.main;
        }

        private void Start()
        {
            playerTransform = PlayerManager.player.transform;
        }

        private void FixedUpdate()
        {
            UpdateHPBar();

            // 플레이어의 위치를 UI 좌표로 변환
            Vector3 worldPosition = playerTransform.position + offset;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            sliderRectTransform.position = screenPosition;
        }

        private void UpdateHPBar()
        {
            float currentHp = _playerData.hp;
            float maxHp = _playerData.maxHp;

            hpSlider.value = currentHp / maxHp;
        }
    }
}
