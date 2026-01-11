using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.Panels
{
    public class CharacterProfile : MonoBehaviour
    {
        //==================================================================
        // 캐릭터 프로필 이미지
        Image characterImage;
        
        [SerializeField][ReadOnly] List<Sprite> sprites = new List<Sprite>();

        //==================================================================
        // 이미지를 가져올 Resources 경로
        string path = "Sprites/illustration/Profile/";

        //==================================================================
        // 사용할 데이터
        private PlayerData _playerData;
        public PlayerData PlayerData
        {
            get { return _playerData; }
            set { _playerData = value; }
        }

        //==================================================================

        private void Awake()
        {
            characterImage = GetComponent<Image>();
            SetProfile();
        }

        public void SetProfile()
        {
            // 여기 다시 하면 됨
            sprites.Add(Resources.Load<Sprite>(path + "Mage_Profile_Panel"));
            sprites.Add(Resources.Load<Sprite>(path + "Warrior_Profile_Panel"));
            sprites.Add(Resources.Load<Sprite>(path + "Assassin_Profile_Panel"));
            
            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            characterImage.sprite = sprites[playerClass];
        }
    }
}


