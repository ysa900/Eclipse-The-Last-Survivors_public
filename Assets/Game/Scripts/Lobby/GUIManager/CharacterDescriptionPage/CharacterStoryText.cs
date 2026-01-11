namespace Eclipse.Lobby
{
    public class CharacterStoryText : Eclipse.CustomText
    {
        string[] descriptions = new string[3];

        private void Awake()
        {
            descriptions[0] = "어둠과 혼란에 빠진 세상에서 태어난 앨리스는 불, 얼음, 번개의 세 가지 원소 마법을 자유자재로 다루는 마법사입니다. "
           + "그녀는 '원소 공명'이라는 경지에 도달할 수 있는 특별한 재능을 지녔습니다. "
           + "앨리스는 사람들에게 희망을 주고 세상에 빛을 되찾기 위해 타락한 천사 벨리알에 맞서 싸웁니다.";
            descriptions[1] = "베르타는 빛과 광기, 어둠의 힘들 다루는 전사입니다. "
            + "본디, 정의로운 성기사였던 그녀는 벨리알의 악한 기운으로 인해 내면이 광기와 어둠에 물들었습니다. "
            + "그러나 그 힘을 자신의 것으로 삼아 싸우며, 세상에 다시 빛과 평화를 되찾기 위해 벨리알에 맞서고 있는 베르타는 지금, 진정한 전사의 길을 걷고 있습니다.";
            descriptions[2] = "카이는 '지키는 자'라는 호칭을 계승한 10대 전사로, 마을을 지키기 위해 금기를 깨고 비급을 개방했습니다. "
            + "이로써 가장 완전한 '지키는 자'로 거듭난 그는, 벨리알을 처치하여 모두를 지키기 위해 길을 떠납니다.";
        }

        public void ChangeCharacterStory(int playerClass)
        {
            Text.text = descriptions[playerClass];
        }
    }
}
