using TMPro;
using UnityEngine;

namespace Eclipse.Game.Panels
{
    public class Timer : MonoBehaviour
    {
        private TextMeshProUGUI timeText;
        private StageManager _stageManager;

        public StageManager StageManager
        {
            get { return _stageManager; }
            set { _stageManager = value; }
        }

        void Awake()
        {
            timeText = GetComponent<TextMeshProUGUI>();
        }

        private void LateUpdate()
        {
            // StageManager가 null일 경우를 대비한 방어 로직 추가
            if (_stageManager != null)
            {
                float playTime = _stageManager.gameTime;
                int min = Mathf.FloorToInt(playTime / 60);
                int sec = Mathf.FloorToInt(playTime % 60);

                // String Interpolation 사용으로 가독성 향상
                timeText.text = $"{min:D2}:{sec:D2}";
            }
            else
            {
                // StageManager가 할당되지 않았음. GUIManager에서 할당하는지 확인 필요
                Debug.LogWarning("StageManager is not assigned.");
            }
        }
    }
}
