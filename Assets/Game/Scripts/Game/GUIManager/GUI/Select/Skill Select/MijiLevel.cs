using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.SkillSelect
{
    public class MijiLevel : MonoBehaviour
    {
        Image[] levelObects;

        private void Awake()
        {
            levelObects = GetComponentsInChildren<Image>();
        }
    }
}