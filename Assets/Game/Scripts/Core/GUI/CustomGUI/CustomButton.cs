using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse
{
    public class CustomButton : CustomGUI, IPointerClickHandler
    {
        protected Button button;
        public Action onClick;
        public Action onRightClick;

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
        }

        // EventSystem이 클릭 감지시 호출함 (IPointerClickHandler 인터페이스 구현)
        public void OnPointerClick(PointerEventData eventData)
        {
            // interactable == false인 버튼도 OnPointerClick은 호출될 수 있기에 검사 필요함.
            if (!button.interactable)
                return;

            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    if (onClick != null)
                    {
                        onClick.Invoke();

                        PlayClickSound();
                    }
                    break;
                case PointerEventData.InputButton.Right:
                    if (onRightClick != null)
                    {
                        onRightClick.Invoke();

                        PlayClickSound();
                    }
                    break;
            }
        }

        private void PlayClickSound()
        {
            if (Game.AudioManager.instance != null)
            {
                Game.AudioManager.instance.PlaySfx(Game.AudioManager.Sfx.Select);
            }
            else if (Lobby.AudioManager.instance != null)
            {
                Lobby.AudioManager.instance.PlaySfx((int)Lobby.AudioManager.Sfx.Select);
            }
        }

        public void MakeInteractable()
        {
            button.interactable = true;
        }

        public void MakeUnInteractable()
        {
            button.interactable = false;
        }
    }
}
