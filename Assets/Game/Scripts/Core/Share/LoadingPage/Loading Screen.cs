using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse
{
    public class LoadingScreen : Eclipse.Screen
    {
        Image characterImage;
        RectTransform rectTransform;

        List<Sprite> sprites = new List<Sprite>();

        protected override void Awake()
        {
            base.Awake();

            characterImage = GetComponentInChildren<CharacterImage>().GetComponent<Image>();
            rectTransform = GetComponentInChildren<CharacterImage>().GetComponentInChildren<RectTransform>();
        }

        public void Init()
        {
            string path = "Sprites/illustration/Profile/";
            sprites.Add(Resources.Load<Sprite>(path + "Mage_Profile"));
            sprites.Add(Resources.Load<Sprite>(path + "Warrior_Profile"));
            sprites.Add(Resources.Load<Sprite>(path + "Assassin_Profile"));

            int playerClass = PlayerPrefs.GetInt("PlayerClass");

            switch (playerClass)
            {
                case 0:
                    rectTransform.sizeDelta = new Vector2(456.75f, 576f);
                    break;
                case 1:
                    rectTransform.sizeDelta = new Vector2(399.7f, 537.6f);
                    break;
                case 2:
                    rectTransform.sizeDelta = new Vector2(549.9f, 998.4f);
                    break;
            }

            characterImage.GetComponent<Image>().sprite = sprites[playerClass];

        }
    }

}