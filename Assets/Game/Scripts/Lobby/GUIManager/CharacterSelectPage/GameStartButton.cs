namespace Eclipse.Lobby
{
    public class GameStartButton : CustomButton
    {
        void OnEnable()
        {
            button.interactable = false;
        }
    }
}