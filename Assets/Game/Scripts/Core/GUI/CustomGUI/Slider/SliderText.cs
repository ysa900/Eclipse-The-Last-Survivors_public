using UnityEngine;
using TMPro;

namespace Eclipse
{
    public class SliderText : CustomText
    {
        public void SetLabelText(float value)
        {
            Text.text = (value * 100).ToString("F0");
        }
    }
}
