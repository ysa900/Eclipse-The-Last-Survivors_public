using UnityEngine;

namespace Eclipse.Game
{
    public class ReviveEffect : Object
    {
        public Transform playerTransform;

        private void Update()
        {
            transform.position = playerTransform.position;
        }
    }
}