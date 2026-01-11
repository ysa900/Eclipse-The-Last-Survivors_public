using UnityEngine;
using UnityEngine.UI;

namespace Eclipse
{
    public class FullScreenToggle : MonoBehaviour
    {
        Toggle _toggle;
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

        public void SetFullScreen(bool isFullscreen)
        {
            _toggle.isOn = isFullscreen;

            if (isFullscreen)
            {
                UnityEngine.Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                UnityEngine.Screen.SetResolution(1920, 1080, true);
            }
            else
            {
                UnityEngine.Screen.fullScreenMode = FullScreenMode.Windowed;
                UnityEngine.Screen.SetResolution(1280, 720, false);
            }

            UnityEngine.Screen.fullScreen = isFullscreen;
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

