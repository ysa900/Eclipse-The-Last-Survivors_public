using UnityEngine;
using UnityEngine.UI;

namespace Eclipse
{
    public class CustomImage : CustomGUI
    {
        private Image _image;
        public Image Image
        {
            get
            {
                if (_image == null) // _image가 null일 때만 GetComponent로 가져옴
                {
                    _image = GetComponent<UnityEngine.UI.Image>();
                }
                return _image;
            }
        }

        public void SetSprite(Sprite sprite)
        {
            Image.sprite = sprite;
        }
    }
}
