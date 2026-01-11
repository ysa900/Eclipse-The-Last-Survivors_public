using System.Collections;

namespace Eclipse.Splash.Splash0
{
    public class LoadingScreen : Eclipse.LoadingScreen
    {
        public override IEnumerator Out()
        {
            gameObject.SetActive(false);
            isTransitioning = false;
            yield break;
        }
    }
}