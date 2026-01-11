using UnityEngine;
using UnityEngine.EventSystems;

namespace Eclipse
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [ReadOnly] public string tooltipContent;
        public bool IsHovered { get; private set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered = true;
            TooltipPanel.Instance.Show(tooltipContent);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;
            TooltipPanel.Instance.Hide();
        }

        public void Refresh()
        {
            if (IsHovered)
            {
                TooltipPanel.Instance.Show(tooltipContent); // 다시 표시
            }
        }
    }
}