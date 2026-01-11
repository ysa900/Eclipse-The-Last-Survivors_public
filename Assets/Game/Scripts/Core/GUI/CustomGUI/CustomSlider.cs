using UnityEngine;
using UnityEngine.UI;

namespace Eclipse
{
    public class CustomSlider : Eclipse.CustomGUI
    {
        public System.Action<float> onValueChanged;
        private Slider _slider;

        private void OnEnable()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        // 슬라이더 값 변경 시 호출되는 함수
        private void OnSliderValueChanged(float value)
        {
            onValueChanged?.Invoke(value); // onValueChanged 델리게이트 호출
        }

        // 슬라이더 값을 설정하는 메서드
        public void SetValue(float value)
        {
            _slider.value = value;
        }

        // 슬라이더 현재 값을 반환하는 메서드
        public float GetValue()
        {
            return _slider.value;
        }

        // 슬라이더 이벤트 리스너 해제
        public void RemoveListeners()
        {
            _slider.onValueChanged.RemoveAllListeners();
        }
    }
}

