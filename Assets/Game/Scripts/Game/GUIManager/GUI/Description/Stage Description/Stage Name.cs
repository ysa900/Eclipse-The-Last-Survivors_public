using TMPro;
using UnityEngine;

namespace Eclipse.Game.StageDescription
{
    public class StageName : MonoBehaviour
    {
        string[] names = new string[3];
        TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
            names[0] = "벨리알의 정원";
            names[1] = "벨리알의 성";
            names[2] = "성 심층부";
        }

        public void ChangeImage(int sceneIndex)
        {
            text.text = names[sceneIndex];
        }
    }
}

