using TMPro;
using UnityEngine;

namespace Eclipse.Game.Panels
{
    public class GoldText : Eclipse.CustomText
    {
        TextMeshProUGUI goldText;

        private Server_PlayerData _server_PlayerData;

        public Server_PlayerData Server_PlayerData
        {
            get { return _server_PlayerData; }
            set { _server_PlayerData = value; }
        }

        private void Awake()
        {
            goldText = GetComponent<TextMeshProUGUI>();
        }

        private void LateUpdate()
        {
            // String Interpolation 사용으로 가독성 향상
            goldText.text = $"{_server_PlayerData.coin}G";
        }
    }
}
