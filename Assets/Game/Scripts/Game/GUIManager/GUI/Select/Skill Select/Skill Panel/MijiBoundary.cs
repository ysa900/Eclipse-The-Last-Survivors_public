using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.SkillSelect
{
    public class MijiBoundary : MonoBehaviour
    {
        Image _boundaryImage;

        private void Awake()
        {
            _boundaryImage = GetComponent<Image>();
        }

        private void Start()
        {
            _boundaryImage.color = Color.white;
        }
    }
}