using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.SkillSelect
{
    public class Boundary : MonoBehaviour
    {
        Image _boundaryImage;

        private void Awake()
        {
            _boundaryImage = GetComponent<Image>();
        }
    }
}