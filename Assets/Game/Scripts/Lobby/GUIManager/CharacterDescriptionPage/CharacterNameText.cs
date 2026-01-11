
namespace Eclipse.Lobby
{
    public class CharacterNameText : Eclipse.CustomText
    {
        string[] names = new string[3];

        private void Awake()
        {
            names[0] = "마법사";
            names[1] = "전사";
            names[2] = "도적";
        }

        public void ChangeCharacterName(int playerClass)
        {
            Text.text = names[playerClass];
        }
    }
}
