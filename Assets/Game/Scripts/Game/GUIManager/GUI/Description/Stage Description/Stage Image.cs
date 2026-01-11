using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.StageDescription
{
    public class StageImage : MonoBehaviour
    {
        Sprite[] sprites = new Sprite[3];
        Image image;

        private void Awake()
        {
            image = GetComponent<Image>();

            string path = "Sprites/UI/Stage Description Panel/";
            sprites[0] = Resources.Load<Sprite>(path + "Stage1/Stage1");
            sprites[1] = Resources.Load<Sprite>(path + "Stage2/Stage2");
            sprites[2] = Resources.Load<Sprite>(path + "Stage3/Stage3");
        }

        public void ChangeImage(int sceneIndex)
        {
            image.sprite = sprites[sceneIndex];
        }
    }
}

