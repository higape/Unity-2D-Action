using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace MyGame
{
    public sealed class AngryPig : Character
    {
        [Tooltip("巡逻范围")]
        public Vector2 patrolRange = new(-1f, 1f);

        [Tooltip("视野")]
        public Vector2 viewRange;

        public float blankTime = 1.5f;

        private Action CurrentAction { get; set; }
        private IEnumerator TempCoroutine { get; set; }

        private void Start()
        {
            GoToIdle();
        }

        private void Update()
        {
            UpdateInvalidTime();
            CurrentAction?.Invoke();
            ApplyAddedVelocity();
        }

        private void GoToIdle()
        {
            Animator.Play(IdleHash);
            CurrentAction = IdleUpdate;
            TempCoroutine = DelayAction(GoToWalk, blankTime);
            StartCoroutine(TempCoroutine);
        }

        private void IdleUpdate()
        {
            NoMove();
            // 检测视野内玩家
            if (CheckViewRange())
            {
                StopCoroutine(TempCoroutine);
                GoToRun();
            }
        }

        private void GoToWalk()
        {
            Animator.Play(WalkHash);
            CurrentAction = WalkUpdate;
            FaceBack();
            TempCoroutine = DelayAction(GoToIdle, blankTime);
            StartCoroutine(TempCoroutine);
        }

        private void WalkUpdate()
        {
            if (IsFaceRight)
            {
                WalkToRight();
            }
            else
            {
                WalkToLeft();
            }

            if (CheckViewRange())
            {
                StopCoroutine(TempCoroutine);
                GoToRun();
            }
        }

        private void GoToRun()
        {
            Animator.Play(RunHash);
            CurrentAction = RunUpdate;
        }

        private void RunUpdate()
        {
            if (CheckViewRange())
            {
                if (IsFaceRight)
                {
                    RunToRight();
                }
                else
                {
                    RunToLeft();
                }
            }
            else
            {
                NoMove();
                GoToIdle();
            }
        }

        private bool CheckViewRange()
        {
            if (IsFaceRight)
            {
                return CheckRangeRight(viewRange, PlayerLayer, ViewPlayer);
            }
            else
            {
                return CheckRangeLeft(viewRange, PlayerLayer, ViewPlayer);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (UnityEditor.Selection.gameObjects.Contains(gameObject))
            {
                Gizmos.color = Color.red;
                if (TryGetComponent<BoxCollider2D>(out var collider))
                {
                    if (IsFaceRight)
                    {
                        GizmosDrawRangeRight(collider, viewRange);
                    }
                    else
                    {
                        GizmosDrawRangeLeft(collider, viewRange);
                    }
                }
            }
        }
#endif
    }
}
