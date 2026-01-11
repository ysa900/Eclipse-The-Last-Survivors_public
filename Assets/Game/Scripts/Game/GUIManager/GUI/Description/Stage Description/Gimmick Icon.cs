using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.StageDescription
{
    public class GimmickIcon : MonoBehaviour
    {
        Sprite[] sprites = new Sprite[3];
        Image image;

        private void Awake()
        {
            image = GetComponent<Image>();

            string path = "Sprites/UI/Stage Description Panel/";
            sprites[0] = Resources.Load<Sprite>(path + "Stage1/Stage1_Gimmick_Icon");
            sprites[1] = Resources.Load<Sprite>(path + "Stage2/Stage2_Gimmick_Icon");
            sprites[2] = Resources.Load<Sprite>(path + "Stage3/Stage3_Gimmick_Icon");
        }

        public void ChangeImage(int sceneIndex)
        {
            image.sprite = sprites[sceneIndex];
        }
    }
}

