using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class MovingPlatform : MonoBehaviour
    {
        public float moveSpeed = 2;
        public Vector2[] displacement;
        public bool loop = true;

        private Rigidbody2D Rigidbody { get; set; }
        private int DisplacementIndex { get; set; }
        private Vector2 CurrentDisplacement => displacement[DisplacementIndex];
        private List<Character> Characters { get; set; } = new();
        private Vector2 TargetPosition { get; set; }
        private Vector2 CurrentNormalized { get; set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            TargetPosition = Rigidbody.position + CurrentDisplacement;
            CurrentNormalized = CurrentDisplacement.normalized;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent<Character>(out var c) && !c.IsFlying)
            {
                Characters.Add(c);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent<Character>(out var c))
            {
                Characters.Remove(c);
            }
        }

        private void Update()
        {
            float velocityX = CurrentNormalized.x * moveSpeed;
            float velocityY = CurrentNormalized.y * moveSpeed;
            Vector2 lastPosition = Rigidbody.position;
            Vector2 positionThisFrame = Vector3.MoveTowards(
                lastPosition,
                TargetPosition,
                moveSpeed * Time.deltaTime
            );

            // 更新平台位置
            Rigidbody.MovePosition(positionThisFrame);

            // 平台横向移动
            if (velocityX != 0)
            {
                foreach (var c in Characters)
                {
                    if (c.IsOnMovingPlatform)
                        // 赋予横向速度，使角色随平台移动
                        c.AddedVelocityX += velocityX;
                }
            }

            // 平台下落
            if (velocityY < 0)
            {
                foreach (var c in Characters)
                {
                    if (c.IsOnMovingPlatform)
                    {
                        if (c.IsStartedJumpThisFrame)
                        {
                            // 消除角色被平台赋予的下落速度
                            c.Rigidbody.velocity = new Vector2(
                                c.Rigidbody.velocity.x,
                                c.Rigidbody.velocity.y - velocityY
                            );
                        }
                        else if (!c.IsJumping)
                        {
                            // 使角色在平台下降时贴合平台
                            c.Rigidbody.velocity = new Vector2(
                                c.Rigidbody.velocity.x,
                                c.Rigidbody.velocity.y + velocityY
                            );
                        }
                    }
                }
            }
            // 平台上升
            else if (velocityY > 0)
            {
                foreach (var c in Characters)
                {
                    if (c.IsOnMovingPlatform)
                    {
                        // 消除角色的向上的力，防止角色脱离平台，同时使角色保持正常的跳跃力
                        c.Rigidbody.velocity = new Vector2(
                            c.Rigidbody.velocity.x,
                            c.Rigidbody.velocity.y - velocityY
                        );
                    }
                }
            }

            // 到达目标位置
            if (positionThisFrame == TargetPosition)
            {
                // 计算相对位移
                Vector2 positionD = positionThisFrame - lastPosition;
                foreach (var c in Characters)
                {
                    if (!c.IsJumping && c.IsOnMovingPlatform)
                    {
                        // 矫正角色位置，因为物理计算存在误差
                        c.Rigidbody.MovePosition(c.Rigidbody.position + positionD);
                    }
                }

                // 准备下一个位置
                DisplacementIndex++;
                if (DisplacementIndex >= displacement.Length)
                {
                    DisplacementIndex = 0;
                    if (!loop)
                        enabled = false;
                }
                TargetPosition = positionThisFrame + CurrentDisplacement;
                CurrentNormalized = CurrentDisplacement.normalized;
            }
        }
    }
}
