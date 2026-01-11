using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse
{
    public class HoverEffect_ForButton : HoverEffect
    {
        Button button;

        protected override void Start()
        {
            base.Start();
            button = GetComponent<Button>();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (button != null && !button.interactable) return;
            base.OnPointerEnter(eventData);
        }
    }
}