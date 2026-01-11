using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.StageDescription
{
    public class AppearingEnemies : MonoBehaviour
    {
        //==================================================================
        Image[] images;

        //==================================================================
        List<Sprite> stage1_enemies = new List<Sprite>();
        List<Sprite> stage2_enemies = new List<Sprite>();
        List<Sprite> stage3_enemies = new List<Sprite>();

        //==================================================================
        List<List<Sprite>> enemies = new List<List<Sprite>>();

        //==================================================================

        private void Awake()
        {
            //==================================================================
            images = GetComponentsInChildren<Image>();
            images = images.Where(img => img.gameObject != this.gameObject).ToArray(); // 자기 자신의 images는 제외

            //==================================================================
            string path = "Sprites/UI/Stage Description Panel/";
            stage1_enemies.Add(Resources.Load<Sprite>(path + "Stage1/EvilTree"));
            stage1_enemies.Add(Resources.Load<Sprite>(path + "Stage1/Pumpkin"));
            stage1_enemies.Add(Resources.Load<Sprite>(path + "Stage1/Warlock"));
            enemies.Add(stage1_enemies);

            //==================================================================
            stage2_enemies.Add(Resources.Load<Sprite>(path + "Stage2/Skeleton Warrior"));
            stage2_enemies.Add(Resources.Load<Sprite>(path + "Stage2/Skeleton Archer"));
            stage2_enemies.Add(Resources.Load<Sprite>(path + "Stage2/Skeleton Horse"));
            stage2_enemies.Add(Resources.Load<Sprite>(path + "Stage2/Bat"));
            enemies.Add(stage2_enemies);

            //==================================================================
            stage3_enemies.Add(Resources.Load<Sprite>(path + "Stage3/Ghoul"));
            stage3_enemies.Add(Resources.Load<Sprite>(path + "Stage3/Spitter"));
            stage3_enemies.Add(Resources.Load<Sprite>(path + "Stage3/Summoner"));
            stage3_enemies.Add(Resources.Load<Sprite>(path + "Stage3/BloodKing"));
            enemies.Add(stage3_enemies);

            //==================================================================
        }

        public void ChangeImage(int sceneIndex)
        {
            for (int i = 0; i < enemies[sceneIndex].Count; i++)
            {
                images[i].sprite = enemies[sceneIndex][i];
            }

            if (sceneIndex == 0) images[3].gameObject.SetActive(false);
            else images[3].gameObject.SetActive(true);
        }
    }

}
