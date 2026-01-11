using System;
using UnityEngine;

namespace Eclipse.Game
{
    public class PurificationStatue : Statue
    {
        // ======================================================
        // 기믹 관련 변수들
        public bool isStatueInInvertedTriangleGroup;
        public bool isStatuePlacedInside;
        public bool isStatueMoving;
        private float moveSpeed;
        public Vector3[] movePositions;
        public Action onStatuePlacedInside;
        public Action onStatuePlacedOutside;

        public bool isPlayerInRange;
        // ======================================================

        protected override void Awake()
        {
            base.Awake();
            moveSpeed = 1f;
            movePositions = new Vector3[2];
        }

        private void Start()
        {
            movePositions[0] = transform.position;
        }

        private void Update()
        {
            DetectStatueMoveInput();

            if (isStatueMoving)
            {
                MoveStatue();

                int sign = isStatuePlacedInside ? 0 : 1;

                float sqrDistance = (movePositions[sign] - transform.position).sqrMagnitude;
                bool isStatueArrivedToDestination = sqrDistance < 0.01f * 0.01f;
                if (isStatueArrivedToDestination)
                {
                    isStatueMoving = false;
                    isStatuePlacedInside = !isStatuePlacedInside;
                    InvokeArrivingAction();
                }
            }
        }

        private void DetectStatueMoveInput()
        {
            if (isPlayerInRange && !isStatueMoving)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    isStatueMoving = true;
                }
            }
        }

        private void MoveStatue()
        {
            // 석상 움직임 구현
            int sign = isStatuePlacedInside ? 0 : 1;
            Vector2 direction = (movePositions[sign] - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * moveSpeed);

            PlayStatueMovingSound();
        }

        private void PlayStatueMovingSound()
        {
            // 석상 움직임 사운드 구현

        }

        private void InvokeArrivingAction()
        {
            if (isStatuePlacedInside)
            {
                onStatuePlacedInside?.Invoke();
            }
            else
            {
                onStatuePlacedOutside?.Invoke();
            }
        }
    }
}