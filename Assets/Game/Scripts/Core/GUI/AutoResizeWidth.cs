using TMPro;
using UnityEngine;

namespace Eclipse
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class AutoResizeWidth : MonoBehaviour
    {
        private TextMeshProUGUI textMesh;
        private RectTransform rectTransform;

        void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void Refresh()
        {
            // 텍스트 내용이 바뀔 때만 계산하도록 최적화 가능
            float preferredWidth = textMesh.GetPreferredValues(textMesh.text).x;

            // RectTransform의 width 조절
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredWidth);
        }
    }
}