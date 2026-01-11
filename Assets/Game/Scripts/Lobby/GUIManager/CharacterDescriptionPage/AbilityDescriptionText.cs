namespace Eclipse.Lobby
{
    public class AbilityDescriptionText : Eclipse.CustomText
    {
        string[] descriptions = new string[3];

        private void Awake()
        {
            descriptions[0] = "특정 두 가지 속성 스킬을 마스터하면,\n두 스킬이 공명하여 새로운 스킬로 발현됩니다.";
            descriptions[1] = "한 가지 계열의 모든 스킬을 마스터하면,\n베르타가 해당 계열로 각성합니다.";
            descriptions[2] = "10레벨마다 비급을 획득해 다양한 전략을\n구사할 수 있습니다.";
        }

        public void ChangeAbilityDescription(int playerClass)
        {
            Text.text = descriptions[playerClass];
        }
    }
}
