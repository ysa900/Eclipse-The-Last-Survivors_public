namespace Eclipse.Splash.Splash3
{
    public class GUIManager : Splash.GUIManager { 
        protected override void Awake()
        {
            base.Awake();

            arrowImage[0] = GetWidget<ArrowImage1>();
            arrowImage[0].Hide();

        }
    }

}
