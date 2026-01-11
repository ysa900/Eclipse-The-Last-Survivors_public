using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.SkillSelect
{
    public class Level : MonoBehaviour
    {
        Image[] levelObects;

        private void Awake()
        {
            levelObects = GetComponentsInChildren<Image>();
        }

        private void Start()
        {
            int length = levelObects.Length;
            if (length != 10) Debug.LogWarning("LevelObjects's length is not 10!");

            for (int i = length / 2; i < length; i++)
                levelObects[i].gameObject.SetActive(false);
        }


    }
}

