
using UnityEngine;

namespace Eclipse.Lobby
{
    public class CharacterPanelImage : Eclipse.CustomImage
    {
        Sprite[] sprites = new Sprite[3];
        private void Awake()
        {
            string path = "Sprites/illustration/Profile/Used Source/";
            sprites[0] = Resources.Load<Sprite>(path + "Mage_Panel_Image");
            sprites[1] = Resources.Load<Sprite>(path + "Warrior_Panel_Image");
            sprites[2] = Resources.Load<Sprite>(path + "Assassin_Panel_Image");
        }

        public void ChangeCharacterPanelImage(int playerclass)
        {
            Image.sprite = sprites[playerclass];
        }

    }
}