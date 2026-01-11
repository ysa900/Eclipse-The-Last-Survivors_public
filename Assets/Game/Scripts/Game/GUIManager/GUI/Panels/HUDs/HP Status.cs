using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.Panels
{
    public class HPStatus : MonoBehaviour
    {
        TextMeshProUGUI hpText;
        Slider hpSlider;

        private PlayerData _playerData;
        public PlayerData PlayerData
        {
            get { return _playerData; }
            set { _playerData = value; }
        }

        private void Awake()
        {
            hpText = GetComponentInChildren<TextMeshProUGUI>();
            hpSlider = GetComponent<Slider>();
        }

        private void LateUpdate()
        {
            float currentHp = _playerData.hp;
            float maxHp = _playerData.maxHp;

            hpText.text = string.Format("{0} / {1}", (int)currentHp, (int)maxHp);
            hpSlider.value = currentHp / maxHp;
        }
    }
}
