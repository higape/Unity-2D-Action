using System;
using System.Collections;
using UnityEngine;

namespace MyGame
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
    public abstract class Character : MonoBehaviour, IHit
    {
        public const float GroundCastDifference = 1f / 32f;
        public const float CastDifference = 1f / 32f;
        public const float JumpHeightRate = 1.05f;
        public const int GroundLayer = 1 << 6;
        public const int SlickGroundLayer = 1 << 7;
        public const int MovingPlatformLayer = 1 << 8;
        public const int PlayerLayer = 1 << 9;
        public const int EnemyLayer = 1 << 10;
        public const int AllGround = GroundLayer | SlickGroundLayer | MovingPlatformLayer;
        public const int ViewPlayer = PlayerLayer | AllGround;
        public static int AppearingHash = Animator.StringToHash("Appearing");
        public static int DesappearingHash = Animator.StringToHash("Desappearing");
        public static int IdleHash = Animator.StringToHash("Idle");
        public static int WalkHash = Animator.StringToHash("Walk");
        public static int RunHash = Animator.StringToHash("Run");
        public static int JumpHash = Animator.StringToHash("Jump");
        public static int FallHash = Animator.StringToHash("Fall");
        public static int HitHash = Animator.StringToHash("Hit");
        public static int AttackHash = Animator.StringToHash("Attack");
        public static int IsLurkHash = Animator.StringToHash("IsLurk");

        public bool isFaceRight = false;
        public float moveSpeed = 2;
        public int hp = 1;
        public int attack = 1;
        public bool hasCollisionDamage = true;
        public float lurkTime;

        public Rigidbody2D Rigidbody { get; protected set; }
        public BoxCollider2D Collider { get; protected set; }
        public Animator Animator { get; protected set; }
        public bool IsFaceRight => isFaceRight;
        public bool CanStopJump { get; set; } = true;
        public bool IsFlying => Rigidbody.gravityScale == 0;
        public float AddedVelocityX { get; set; }
        public float CurrentSelfMoveSpeed => moveSpeed;
        public virtual int Hp
        {
            get => hp;
            protected set { hp = Mathf.Max(0, value); }
        }
        public bool IsSelfMoving { get; protected set; }
        public bool IsMoving => IsSelfMoving || AddedVelocityX != 0;
        public bool IsStoppedMovementThisFrame { get; protected set; }
        public bool IsJumping { get; protected set; }
        public bool IsStartedJumpThisFrame { get; protected set; }
        public Vector2 Position => Rigidbody.position;
        public bool IsOnSlickGround => GroundCast(SlickGroundLayer);
        public bool IsOnMovingPlatform => GroundCast(MovingPlatformLayer);
        public float LurkTimeCount { get; protected set; }

        protected void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<BoxCollider2D>();
            Animator = GetComponent<Animator>();
        }

        protected void OnCollisionStay2D(Collision2D other)
        {
            if (hasCollisionDamage && other.gameObject.TryGetComponent<Character>(out var c))
            {
                c.Hit(attack, (c.Position - Position).normalized);
            }
        }

        protected IEnumerator DelayAction(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action();
        }

        protected void SetInvalidTime()
        {
            if (lurkTime > 0)
            {
                LurkTimeCount = lurkTime;
                Animator.SetBool(IsLurkHash, true);
            }
        }

        protected void UpdateInvalidTime()
        {
            if (LurkTimeCount > 0)
            {
                LurkTimeCount -= Time.deltaTime;
                if (LurkTimeCount < 0)
                    Animator.SetBool(IsLurkHash, false);
            }
        }

        public virtual void Hit(int attack, Vector2 direction)
        {
            if (LurkTimeCount <= 0 && Hp > 0)
            {
                Hp -= attack;
                SetInvalidTime();
                Animator.SetTrigger(HitHash);
            }
        }

        // animation event
        protected virtual void OnHitEnd()
        {
            if (Hp <= 0)
                Destroy(gameObject);
        }

        public virtual void Dead()
        {
            Destroy(gameObject);
        }

        protected void FaceLeft()
        {
            if (isFaceRight)
            {
                transform.Rotate(0, 180, 0);
                isFaceRight = false;
            }
        }

        protected void FaceRight()
        {
            if (!isFaceRight)
            {
                transform.Rotate(0, 180, 0);
                isFaceRight = true;
            }
        }

        protected void FaceBack()
        {
            transform.Rotate(0, 180, 0);
            isFaceRight = !isFaceRight;
        }

        protected void WalkToLeft()
        {
            IsSelfMoving = true;
            Rigidbody.velocity = new Vector2(-moveSpeed, Rigidbody.velocity.y);
            FaceLeft();
        }

        protected void WalkToRight()
        {
            IsSelfMoving = true;
            Rigidbody.velocity = new Vector2(moveSpeed, Rigidbody.velocity.y);
            FaceRight();
        }

        protected void RunToLeft()
        {
            IsSelfMoving = true;
            Rigidbody.velocity = new Vector2(-moveSpeed * 2, Rigidbody.velocity.y);
            FaceLeft();
        }

        protected void RunToRight()
        {
            IsSelfMoving = true;
            Rigidbody.velocity = new Vector2(moveSpeed * 2, Rigidbody.velocity.y);
            FaceRight();
        }

        protected void NoMove()
        {
            Rigidbody.velocity = new Vector2(0, Rigidbody.velocity.y);
            if (IsSelfMoving)
                IsStoppedMovementThisFrame = true;
            else
                IsStoppedMovementThisFrame = false;
            IsSelfMoving = false;
        }

        protected void ApplyAddedVelocity()
        {
            if (AddedVelocityX != 0)
            {
                Rigidbody.velocity = new Vector2(
                    Rigidbody.velocity.x + AddedVelocityX,
                    Rigidbody.velocity.y
                );
                AddedVelocityX = 0;
            }
        }

        protected void BulletCallback(Collider2D other, Vector2 direction)
        {
            if (other.TryGetComponent<IHit>(out var character))
            {
                character.Hit(attack, direction);
            }
        }

        protected bool GroundCast(int layerMask)
        {
            var b = Collider.bounds;
            return Physics2D.BoxCast(
                b.center,
                new(
                    Collider.size.x + Collider.edgeRadius * 2 - GroundCastDifference,
                    Collider.size.y + Collider.edgeRadius * 2
                ),
                0,
                Vector2.down,
                CastDifference,
                layerMask
            );
        }

        protected bool CheckRaycast(Bounds own, Collider2D other, int targetLayer, int layerMask)
        {
            if (other)
            {
                var cast = Physics2D.Raycast(
                    own.center,
                    other.bounds.center - own.center,
                    float.PositiveInfinity,
                    layerMask
                );
                if (
                    cast.collider
                    && Common.ContainLayer(targetLayer, cast.collider.gameObject.layer)
                )
                    return true;
            }
            return false;
        }

        protected bool CheckRangeDown(Vector2 size, int targetLayer, int layerMask)
        {
            var b = Collider.bounds;
            var collider = Physics2D.OverlapBox(
                new Vector2(
                    b.center.x,
                    b.center.y - b.extents.y - Collider.edgeRadius - size.y / 2 - CastDifference
                ),
                size,
                0,
                targetLayer
            );
            return CheckRaycast(b, collider, targetLayer, layerMask);
        }

        protected bool CheckRangeLeft(Vector2 size, int targetLayer, int layerMask)
        {
            var b = Collider.bounds;
            var collider = Physics2D.OverlapBox(
                new Vector2(
                    b.center.x - b.extents.x - Collider.edgeRadius - size.x / 2 - CastDifference,
                    b.center.y
                ),
                size,
                0,
                targetLayer
            );
            return CheckRaycast(b, collider, targetLayer, layerMask);
        }

        protected bool CheckRangeRight(Vector2 size, int targetLayer, int layerMask)
        {
            var b = Collider.bounds;
            var collider = Physics2D.OverlapBox(
                new Vector2(
                    b.center.x + b.extents.x + Collider.edgeRadius + size.x / 2 + CastDifference,
                    b.center.y
                ),
                size,
                0,
                targetLayer
            );
            return CheckRaycast(b, collider, targetLayer, layerMask);
        }

#if UNITY_EDITOR
        protected void GizmosDrawRangeDown(BoxCollider2D collider, Vector2 size)
        {
            var b = collider.bounds;
            Gizmos.DrawWireCube(
                new Vector2(
                    b.center.x,
                    b.center.y - b.extents.y - collider.edgeRadius - size.y / 2 - CastDifference
                ),
                size
            );
        }

        protected void GizmosDrawRangeLeft(BoxCollider2D collider, Vector2 size)
        {
            var b = collider.bounds;
            Gizmos.DrawWireCube(
                new Vector2(
                    b.center.x - b.extents.x - collider.edgeRadius - size.x / 2 - CastDifference,
                    b.center.y
                ),
                size
            );
        }

        protected void GizmosDrawRangeRight(BoxCollider2D collider, Vector2 size)
        {
            var b = collider.bounds;
            Gizmos.DrawWireCube(
                new Vector2(
                    b.center.x + b.extents.x + collider.edgeRadius + size.x / 2 + CastDifference,
                    b.center.y
                ),
                size
            );
        }
#endif
    }
}
