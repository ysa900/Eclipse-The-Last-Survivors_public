using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.Panels
{
    public class EnemyHPSlider : MonoBehaviour
    {
        private Slider hpSlider;
        private RectTransform sliderRectTransform;
        TextMeshProUGUI bossHpText;

        private Vector2 offset; // UI 위치 보정용 오프셋
        private Vector2 offset_Statue = new Vector2(0.17f, 1.22f); // Statue UI 위치 보정용 오프셋
        private Vector2 offset_FighterGoblin = new Vector2(0.01f, 0.82f); // FighterGoblin UI 위치 보정용 오프셋
        private Vector2 offset_RuinedKing = new Vector2(0.02f, 0.43f); // RuinedKing UI 위치 보정용 오프셋

        public Enemy enemy;
        private Camera mainCamera; // 메인 카메라

        private void Awake()
        {
            hpSlider = GetComponent<Slider>();
            sliderRectTransform = GetComponent<RectTransform>();
            mainCamera = Camera.main;
            bossHpText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            if (enemy is Statue)
            {
                offset = offset_Statue;
            }
            else if (enemy is FighterGoblin)
            {
                offset = offset_FighterGoblin;
            }
            else if (enemy is RuinedKing)
            {
                offset = offset_RuinedKing;
            }
            else
            {
                offset = Vector2.zero; // 기본 오프셋
            }

            SetScreenPosition(); // 초기 위치 설정
        }

        private void FixedUpdate()
        {
            UpdateHPBar();
            SetScreenPosition();
        }

        private void UpdateHPBar()
        {
            float currentHp = enemy.hp;
            float maxHp = enemy.maxHp;

            hpSlider.value = currentHp / maxHp;
            if (bossHpText != null)
            {
                bossHpText.text = string.Format("{0} / {1}", (int)currentHp, (int)maxHp);
            }
        }

        // Statue의 위치를 UI 좌표로 변환
        private void SetScreenPosition()
        {
            Vector3 worldPosition = (Vector2)enemy.transform.position + offset;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            sliderRectTransform.position = screenPosition;
        }
    }
}
