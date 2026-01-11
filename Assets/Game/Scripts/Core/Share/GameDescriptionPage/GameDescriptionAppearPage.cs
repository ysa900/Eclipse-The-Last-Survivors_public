using UnityEngine;

namespace Eclipse
{
    public class GameDescriptionAppearPage : Eclipse.Viewer
    {
        Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }
}
