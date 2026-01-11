using TMPro;
using UnityEngine;

namespace Eclipse
{
    public class CustomText : CustomGUI
    {
        private TextMeshProUGUI _textMeshPro;
        public TextMeshProUGUI Text
        {
            get
            {
                if (_textMeshPro == null)
                {
                    _textMeshPro = GetComponent<TextMeshProUGUI>();
                }
                return _textMeshPro;
            }
        }

        public void SetText(string text)
        {
            Text.text = text;
        }
    }

}
