using UnityEngine;

namespace Eclipse.Game
{
    public class Object : MonoBehaviour
    {
        public float X
        {
            get
            {
                return transform.position.x;
            }
            set
            {
                Vector2 pos = transform.position;
                transform.position = new Vector2(value, pos.y);
            }
        }

        public float Y
        {
            get
            {
                return transform.position.y;
            }
            set
            {
                Vector2 pos = transform.position;
                transform.position = new Vector2(pos.x, value);
            }
        }

    }
}