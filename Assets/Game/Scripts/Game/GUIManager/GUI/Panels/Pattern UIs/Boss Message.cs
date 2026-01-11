using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Eclipse.Game.Panels
{
    public class BossMessage : MonoBehaviour
    {
        List<string> pattern_messages = new List<string>();
        List<string> gimmick_messages = new List<string>();

        TextMeshProUGUI text;
        Color alpha; // alpha값을 캐싱할 변수
        float alphaSpeed = 0.075f;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();

            pattern_messages.Add("건방진 것... 죽어라...");
            pattern_messages.Add("어리석은 필멸자여... ");
            pattern_messages.Add("가소롭구나..");

            gimmick_messages.Add("나를 지켜라. 제물들이여!!");
            gimmick_messages.Add("힘을 바쳐라. 적에게.. 죽음을!!");
            gimmick_messages.Add("의식을 시작한다.. 방해하게 두진 않겠다.");
            gimmick_messages.Add("안돼..이럴 수는 없다...이럴 수는 없단 말이다!!!");
            gimmick_messages.Add("나는...불멸이다!!!");
        }

        private void OnEnable()
        {
            alphaSpeed = 0.075f;
            text.color = new Color(1, 0, 0, 1);
            alpha = text.color;
        }

        private void LateUpdate()
        {
            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
            if (alpha.a < 0.8) alphaSpeed = 2;
            if (alpha.a < 0.01) alpha.a = 0;
            text.color = alpha;

            if (alpha.a == 0)
            {
                gameObject.SetActive(false);
            }
        }

        public void ChangePatternMessage(int sceneIndex)
        {
            text.text = pattern_messages[sceneIndex];
        }

        public void ChangeGimmickMessage(int sceneIndex)
        {
            text.text = gimmick_messages[sceneIndex];
        }
    }
}