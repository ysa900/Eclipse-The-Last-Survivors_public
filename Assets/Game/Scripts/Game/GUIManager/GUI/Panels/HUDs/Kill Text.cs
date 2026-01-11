using TMPro;
using UnityEngine;

namespace Eclipse.Game.Panels
{
    public class KillText : MonoBehaviour
    {
        TextMeshProUGUI killText;

        private PlayerData _playerData;
        public PlayerData PlayerData
        {
            get { return _playerData; }
            set { _playerData = value; }
        }

        private void Awake()
        {
            killText = GetComponent<TextMeshProUGUI>();
        }

        private void LateUpdate()
        {
            // String Interpolation 사용으로 가독성 향상
            killText.text = $"{_playerData.kill}";
        }
    }
}

