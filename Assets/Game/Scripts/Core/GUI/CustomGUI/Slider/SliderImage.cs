using UnityEngine.UI;

namespace Eclipse
{
    public class SliderImage : CustomImage
    {
        public void SetFillAmount(float fillAmount)
        {
            Image.fillAmount = fillAmount;
        }
    }
}
