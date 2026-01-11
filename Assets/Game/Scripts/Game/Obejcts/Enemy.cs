using UnityEngine;

namespace Eclipse.Game
{
    public class Enemy : Object
    {
        public float hp;
        public float maxHp;
        public float damage;

        public Collider2D col; // Collider의 offset을 변경하기 위한 변수

        protected GameObject targetObject;
        public GameObject TargetObject
        {
            get => targetObject;
            set => targetObject = value;
        }

        protected virtual void Awake()
        {
            col = GetComponent<Collider2D>();
        }
    }
}