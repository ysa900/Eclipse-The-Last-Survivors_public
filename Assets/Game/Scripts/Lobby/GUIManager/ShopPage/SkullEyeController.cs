using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Lobby.Shop
{
    public class SkullEyeController : MonoBehaviour
    {
        Material mat;
        [Range(0,1)] public float glowIntensity = 0f;
        [Range(0,1)] public float crossFlashIntensity = 0f;

        int glowMaxLevel = 3;
        int crossMaxLevel = 2;

        private void Awake()
        {
            mat = GetComponent<Image>().material;
            CustomButton parentButton = transform.parent.GetComponent<CustomButton>();
        }

        public void SetGlowIntensity(int curLevel)
        {
            if (mat != null)
            {
                glowIntensity = Mathf.Clamp01((float)curLevel / glowMaxLevel);
                mat.SetFloat("_GlowIntensity", glowIntensity);

                curLevel -= glowMaxLevel;
                crossFlashIntensity = Mathf.Clamp01((float)curLevel / crossMaxLevel);
                mat.SetFloat("_Level5EffectIntensity", crossFlashIntensity);
            }
        }
    }
}