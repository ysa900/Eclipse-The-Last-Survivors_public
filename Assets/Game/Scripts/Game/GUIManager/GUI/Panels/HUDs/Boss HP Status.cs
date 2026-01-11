using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Eclipse.Game.Panels
{
    public class BossHPStatus : MonoBehaviour
    {
        Slider bossHPSlider;
        TextMeshProUGUI bossHpText;

        private BossManager _bossManager; // GUIManager∞° ≥÷æÓ¡‡æﬂµ 
        public BossManager BossManager
        {
            get { return _bossManager; }
            set { _bossManager = value; }
        }

        private void Awake()
        {
            bossHPSlider = GetComponentInChildren<Slider>();

            if (SceneManager.GetActiveScene().name == "Stage3")
            {
                bossHpText = GetComponentsInChildren<TextMeshProUGUI>()[1];
            }
            else
            {
                bossHpText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        private void LateUpdate()
        {
            if (_bossManager != null)
            {
                float currentBossHp = _bossManager.boss.hp;
                float bossMaxHp = _bossManager.boss.maxHp;

                bossHPSlider.value = currentBossHp / bossMaxHp * 100;
                bossHpText.text = string.Format("{0} / {1}", (int)currentBossHp, (int)bossMaxHp);
            }
            else
            {
                Debug.LogWarning("BossManager is not assigned");
            }
        }
    }
}

