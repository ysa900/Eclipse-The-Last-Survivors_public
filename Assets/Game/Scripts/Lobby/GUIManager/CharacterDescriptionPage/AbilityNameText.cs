
namespace Eclipse.Lobby
{
    public class AbilityNameText : Eclipse.CustomText
    {
        string[] names = new string[3];

        private void Awake()
        {
            names[0] = "원소 공명";
            names[1] = "각성";
            names[2] = "비급";
        }

        public void ChangeAbilityName(int playerClass)
        {
            Text.text = names[playerClass];
        }
    }
}
