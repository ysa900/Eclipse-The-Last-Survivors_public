using TMPro;
using UnityEngine;

namespace Eclipse.Game.Panels
{
    public class PlayerLevel : MonoBehaviour
    {
        TextMeshProUGUI levelText;

        private PlayerData _playerData;
        public PlayerData PlayerData
        {
            get { return _playerData; }
            set { _playerData = value; }
        }

        private void Awake()
        {
            levelText = GetComponent<TextMeshProUGUI>();
        }

        private void LateUpdate()
        {
            // String Interpolation 사용으로 가독성 향상
            levelText.text = $"Lv.{_playerData.level}";
        }
    }
}


