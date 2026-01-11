using UnityEngine;
using UnityEngine.UI;

namespace Eclipse
{
    public class MuteToggle : MonoBehaviour
    {
        private Toggle _toggle;
        public System.Action<bool> onToggleChanged;

        private void OnEnable()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            onToggleChanged?.Invoke(isOn);
        }

        public void SetToggleValue(bool isOn)
        {
            _toggle.isOn = isOn;
            AudioListener.volume = isOn ? 0 : 1;
        }

        public bool GetToggleValue()
        {
            return _toggle.isOn;
        }

        public void RemoveListeners()
        {
            onToggleChanged = null;
            _toggle.onValueChanged.RemoveAllListeners();
        }

        public void Show()
        {
            _toggle.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _toggle.gameObject.SetActive(false);
        }
    }
}