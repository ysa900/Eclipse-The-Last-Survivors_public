
using UnityEngine;

namespace Eclipse.Lobby
{
    public class AbilityImage : Eclipse.CustomImage
    {
        Sprite[] sprites = new Sprite[3];
        private void Awake()
        {
            string path = "Sprites/UI/Lobby & Splash/";
            sprites[0] = Resources.Load<Sprite>(path + "Magician Panel Icon");
            sprites[1] = Resources.Load<Sprite>(path + "Warrior Panel Icon");
            sprites[2] = Resources.Load<Sprite>(path + "Assassin Panel Icon");
        }

        public void ChangeAbilityImage(int playerclass)
        {
            Image.sprite = sprites[playerclass];
        }

    }
}
