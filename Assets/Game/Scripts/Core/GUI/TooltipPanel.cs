using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse
{
    public class TooltipPanel : MonoBehaviour
    {
        public static TooltipPanel Instance;

        [SerializeField] RectTransform paddingContainer; // 인스펙터에서 설정
        Vector2 offset = new Vector2(10f, -10f);
        private Vector2 margin = new Vector2(10, -10f); // 화면 끝 여백

        TextMeshProUGUI tooltipText;
        RectTransform panelRect;
        Canvas canvas;

        AutoResizeWidth autoResizeWidth; // 자동 크기 조절 컴포넌트

        private void Awake()
        {
            Instance = this;
            tooltipText = GetComponentInChildren<TextMeshProUGUI>();
            panelRect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            autoResizeWidth = GetComponentInChildren<AutoResizeWidth>();
        }
        private void Update()
        {
            SetPosition();
        }

        void LateUpdate()
        {
            if (paddingContainer == null) return;

            panelRect.sizeDelta = paddingContainer.sizeDelta;
        }

        private void SetPosition()
        {
            if (!gameObject.activeSelf) return;

            Vector2 anchoredPos;
            Vector2 mousePos = Input.mousePosition;

            // 1. 기본 위치: 마우스 오른쪽 아래
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out anchoredPos
            );

            // 2. 툴팁 패널 크기 고려해서 오른쪽 아래 배치
            Vector2 panelSize = panelRect.sizeDelta;
            anchoredPos += new Vector2(panelSize.x * 0.5f, -panelSize.y * 0.5f);
            anchoredPos += offset; // 오프셋 추가

            // 3. 화면 범위 제한 (툴팁이 밖으로 나가지 않도록)
            RectTransform canvasRect = canvas.transform as RectTransform;

            Vector2 clampedPos = anchoredPos;
            float halfWidth = panelSize.x * 0.5f;
            float halfHeight = panelSize.y * 0.5f;

            float maxX = canvasRect.rect.width * 0.5f - halfWidth - margin.x;
            float minX = -canvasRect.rect.width * 0.5f + halfWidth + margin.x;
            float maxY = canvasRect.rect.height * 0.5f - halfHeight - margin.y;
            float minY = -canvasRect.rect.height * 0.5f + halfHeight + margin.y;

            clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
            clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);

            panelRect.anchoredPosition = clampedPos;
        }

        public void Show(string line)
        {
            tooltipText.text = line;

            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect); // 사이즈 갱신

            autoResizeWidth.Refresh(); // 자동 크기 조절
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}