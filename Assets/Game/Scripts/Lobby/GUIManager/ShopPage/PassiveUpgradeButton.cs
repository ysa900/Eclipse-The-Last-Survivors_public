using UnityEngine;

namespace Eclipse.Lobby.Shop
{
    public class PassiveUpgradeButton : CustomButton
    {
        private CanvasGroup canvasGroup;

        protected override void Awake()
        {
            base.Awake();

            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetTransparent(bool isTransparent)
        {
            canvasGroup.alpha = isTransparent ? 0.5f : 1f;
        }
    }
}