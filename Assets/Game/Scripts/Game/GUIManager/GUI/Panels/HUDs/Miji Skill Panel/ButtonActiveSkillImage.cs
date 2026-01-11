using TMPro;
using UnityEngine;

namespace Eclipse.Game.Panels
{
    public class ButtonActiveSkillImage : Eclipse.CustomImage
    {
        // fillAmount 값을 설정하는 메서드
        public void SetFillAmount(float value)
        {
            // value는 0과 1 사이의 값이어야 함
            Image.fillAmount = Mathf.Clamp01(value);
        }
    }
}

