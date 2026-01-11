using UnityEngine;

namespace Eclipse
{
    public class GameDescriptionNextPage : Eclipse.Viewer
    {
        Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }
}
