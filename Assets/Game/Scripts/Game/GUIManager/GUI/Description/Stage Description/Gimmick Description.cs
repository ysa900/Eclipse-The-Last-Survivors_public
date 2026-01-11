using TMPro;
using UnityEngine;

namespace Eclipse.Game.StageDescription
{
    public class GimmickDescription : MonoBehaviour
    {
        [SerializeField] string[] names = new string[3];
        TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();

            names[0] = "게임 시작 후 2분이 지나면 벨리알의 정원에\n독성 늪이 등장합니다. " +
                "이 독성 늪을 밟으면\n빠르게 체력이 줄어들기 때문에 주의해야\n합니다. " +
                "독성 늪을 피하며 적들과 싸우는 전략이 필요합니다.";
            names[1] = "벨리알의 성에는 적 처치 시 확률정으로 박쥐\n상자가 등장합니다. " +
                "이 상자를 열면 1분간\n박쥐 때의 습격을 받지만, 습격을 버텨내면\n막대한 보상을 얻을 수 있습니다.";
            names[2] = "주기적으로 플레이어 위치에 죽음의 광선을 발사하는 벨리알의 권능이 등장합니다. " +
                "이 광선은 큰 피해를 입히므로 주의해야 합니다.";
        }

        public void ChangeImage(int sceneIndex)
        {
            text.text = names[sceneIndex];
        }
    }
}

