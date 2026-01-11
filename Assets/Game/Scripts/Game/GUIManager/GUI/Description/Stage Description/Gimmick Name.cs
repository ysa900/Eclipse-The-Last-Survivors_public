using TMPro;
using UnityEngine;

namespace Eclipse.Game.StageDescription
{
    public class GimmickName : MonoBehaviour
    {
        string[] names = new string[3];
        TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();

            names[0] = "스테이지 기믹: 독성 늪";
            names[1] = "스테이지 기믹: 박쥐 상자";
            names[2] = "스테이지 기믹: 벨리알의 권능";
        }

        public void ChangeImage(int sceneIndex)
        {
            text.text = names[sceneIndex];
        }
    }
}

