using TMPro;
using UnityEngine;

namespace Eclipse.Game.StageDescription
{
    public class StageDescription : MonoBehaviour
    {
        string[] names = new string[3];
        TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();

            names[0] = "어둠에 물든 벨리알의 정원은 당신이 첫 발을 내딛는\n곳입니다. " +
                "이곳에는 생명력이 뒤틀린 엔트, 으스스한\n잭 오랜턴, 그리고 사악한 워록이 길을 막아섭니다.\n" +
                "당신은 이 혼란 속에서 자신의 힘을 시험하며, 벨리알에게 다가가기 위한 첫 관문을 돌파해야 합니다.";
            names[1] = "벨리알의 성에는 벨리알이 되살린 과거 용사들의 유해가 스켈레톤 병사들로 변해 나타납니다. " +
                "성 심층부에 진입하기 위해서는 이 스켈레톤 병사들을 물리치고, 깊숙한 곳으로 나아가야 합니다.";
            names[2] = "벨리알의 성 심층부에는 어둠이 뭉쳐 만들어진 구울들이 모습을 드러냅니다. " +
                "이들은 어둠 속에서 태어나 강력한\n위협을 가합니다. 당신은 이 존재들을 물리치고, 마침내\n모습을 드러낸" +
                " 개기 일식의 원흉, 벨리알과 맞서 싸워야\n합니다.";
        }

        public void ChangeImage(int sceneIndex)
        {
            text.text = names[sceneIndex];
        }
    }
}

