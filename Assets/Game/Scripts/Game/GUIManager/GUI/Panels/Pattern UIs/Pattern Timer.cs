using TMPro;
using UnityEngine;

namespace Eclipse.Game.Panels
{
    public class PatternTimer : MonoBehaviour
    {
        private TextMeshProUGUI timeText;
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private PatternManager _patternManager;

        public PatternManager PatternManager
        {
            get { return _patternManager; }
            set { _patternManager = value; }
        }

        private BossGimmickController _bossGimmickController;
        public BossGimmickController BossGimmickController
        {
            get { return _bossGimmickController; }
            set { _bossGimmickController = value; }
        }

        void Awake()
        {
            timeText = GetComponent<TextMeshProUGUI>();
        }

        private void LateUpdate()
        {
            // 패턴 주체들이 null일 경우를 대비한 방어 로직 추가
            if (_patternManager != null)
            {
                float playTime = _patternManager.patternTimer;
                SetTimer(playTime);
            }
            else if (_bossGimmickController != null)
            {
                float playTime = _bossGimmickController.gimmickTimer;
                SetTimer(playTime);
            }
            else
            {
                Debug.LogWarning("Anything is not assigned.");
            }
        }

        void SetTimer(float playTime)
        {
            int min = Mathf.FloorToInt(playTime / 60);
            int sec = Mathf.FloorToInt(playTime % 60);

            // String Interpolation 사용으로 가독성 향상
            timeText.text = $"{_text} {min:D2}:{sec:D2}";
        }
    }
}

