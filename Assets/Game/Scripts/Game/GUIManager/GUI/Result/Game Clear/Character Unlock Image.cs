using TMPro;
using UnityEngine;

namespace Eclipse.Game.GameClear
{
    public class CharacterUnlockImage : CustomGUI
    {
        Animator animator;
        public RuntimeAnimatorController warriorController;
        public RuntimeAnimatorController assassinController;
        TextMeshProUGUI characterUnlockText;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            characterUnlockText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void ShowCharacterUnlock(string characterName)
        {
            characterUnlockText.text = $"{characterName} 해금!";
            if (characterName == "전사")
            {
                animator.runtimeAnimatorController = warriorController;
            }
            else if (characterName == "도적")
            {
                animator.runtimeAnimatorController = assassinController;
            }
        }
    }
}