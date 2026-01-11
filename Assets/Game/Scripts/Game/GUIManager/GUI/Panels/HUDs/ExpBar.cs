using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.Panels
{
    public class ExpBar : MonoBehaviour
    {
        Slider expSlider;

        private PlayerData _playerData;
        public PlayerData PlayerData
        {
            get { return _playerData; }
            set { _playerData = value; }
        }

        private void Awake()
        {
            expSlider = GetComponent<Slider>();
        }

        private void LateUpdate()
        {
            float curExp = _playerData.Exp;
            float maxExp = _playerData.nextExp[_playerData.level];

            expSlider.value = curExp / maxExp;
        }
    }
}