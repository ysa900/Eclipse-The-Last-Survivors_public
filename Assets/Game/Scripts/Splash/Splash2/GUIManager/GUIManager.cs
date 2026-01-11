namespace Eclipse.Splash.Splash2
{
    public class GUIManager : Splash.GUIManager
    {
        protected override void Awake()
        {
            base.Awake();

            arrowImage[0] = GetWidget<ArrowImage1>();
            arrowImage[1] = GetWidget<ArrowImage2>();

            arrowImage[0].Hide();
            arrowImage[1].Hide();

        }
    }
}

