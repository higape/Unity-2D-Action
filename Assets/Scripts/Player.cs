using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyGame
{
    public sealed class Player : Character
    {
        private static int score = 0;

        public static Vector2 RebornPosition { get; set; }
        public static bool RebornFaceRight { get; set; }

        public static void AddFruit()
        {
            score++;
            SceneUI.RefreshScore(score);
        }

        public float jumpHeight = 1.5f;
        public float minFallSpeed = -8;
        public LayerMask groundMask;
        public GameObject bulletPrefab;
        public GameObject bulletPoint;
        public float bulletSpeed;

        public PlayerInput PlayerInput { get; private set; }
        private Action CurrentAction { get; set; }
        public bool IsOnGround { get; private set; }
        public Vector2 HitVelocity { get; private set; }
        public int MaxHp { get; private set; }

        public override int Hp
        {
            get => hp;
            protected set
            {
                hp = Mathf.Max(0, value);
                SceneUI.RefreshLife(hp);
            }
        }

        private new void Awake()
        {
            base.Awake();
            PlayerInput = GetComponent<PlayerInput>();
            MaxHp = hp;
        }

        private void Start()
        {
            Hp = MaxHp;
            SceneUI.RefreshScore(score);
            GoToAppearing();
        }

        private void Update()
        {
            IsOnGround = GroundCast(groundMask);
            if (IsOnGround)
                CanStopJump = true;
            UpdateInvalidTime();
            CurrentAction?.Invoke();
            ApplyAddedVelocity();

            if (PlayerInput.actions["Test1"].WasPerformedThisFrame())
            {
                print("Test1");
            }

            if (PlayerInput.actions["Test2"].WasPerformedThisFrame())
            {
                print("Test2");
            }
        }

        private void UpdateAttack()
        {
            if (PlayerInput.actions["Fire"].WasPerformedThisFrame())
            {
                LaunchBullet();
            }
        }

        public override void Hit(int attack, Vector2 direction)
        {
            if (LurkTimeCount <= 0 && Hp > 0)
            {
                Hp -= attack;
                SetInvalidTime();
                HitVelocity = direction * attack;
                GoToHit();
            }
        }

        // animation event
        protected override void OnHitEnd()
        {
            if (Hp <= 0)
                Dead();
            else
                GoToIdle();
        }

        public override void Dead()
        {
            Hp = 0;
            CurrentAction = DeadUpdate;
            Rigidbody.gravityScale = 0;
            Rigidbody.velocity = Vector2.zero;
            Animator.Play(DesappearingHash);
            StartCoroutine(DelayAction(Reborn, 2f));
        }

        private void DeadUpdate()
        {
            Rigidbody.velocity = Vector2.zero;
        }

        public void Reborn()
        {
            Hp = MaxHp;
            transform.position = new Vector3(
                RebornPosition.x,
                RebornPosition.y,
                transform.position.z
            );
            if (RebornFaceRight)
                FaceRight();
            else
                FaceLeft();
            GoToAppearing();
        }

        private void LaunchBullet()
        {
            var bullet = Instantiate(
                    bulletPrefab,
                    bulletPoint.transform.position,
                    Quaternion.identity
                )
                .GetComponent<Bullet>();
            bullet.Setup(
                BulletCallback,
                EnemyLayer | AllGround,
                IsFaceRight ? new Vector2(bulletSpeed, 0) : new Vector2(-bulletSpeed, 0),
                Bullet.DefaultDeadTime
            );
        }

        private float HandleMove()
        {
            var moveValue = PlayerInput.actions["Move"].ReadValue<float>();
            if (moveValue < 0)
                WalkToLeft();
            else if (moveValue > 0)
                WalkToRight();
            else
                NoMove();
            return moveValue;
        }

        private void GoToAppearing()
        {
            Animator.Play(AppearingHash);
            CurrentAction = NoMove;
        }

        // animation event
        private void OnAppearingEnd()
        {
            Rigidbody.gravityScale = 1;
            GoToIdle();
        }

        private void GoToIdle()
        {
            Animator.Play(IdleHash);
            CurrentAction = IdleUpdate;
        }

        private void IdleUpdate()
        {
            UpdateAttack();
            if (!IsOnGround)
            {
                GoToFall();
            }
            else if (PlayerInput.actions["Jump"].WasPerformedThisFrame())
            {
                GoToJump();
            }
            else if (HandleMove() != 0)
            {
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
            UpdateAttack();
            if (!IsOnGround)
            {
                GoToFall();
            }
            else if (PlayerInput.actions["Jump"].WasPerformedThisFrame())
            {
                GoToJump();
            }
            else if (HandleMove() == 0)
            {
                GoToIdle();
            }
        }

        private void GoToJump()
        {
            Vector2 jumpValue =
                new(0, Mathf.Sqrt(-2 * Physics2D.gravity.y * jumpHeight * JumpHeightRate));
            Rigidbody.AddForce(jumpValue, ForceMode2D.Impulse);
            Animator.Play(JumpHash);
            CurrentAction = JumpUpdate;
            IsJumping = true;
            IsStartedJumpThisFrame = true;
        }

        private void JumpUpdate()
        {
            UpdateAttack();
            IsStartedJumpThisFrame = false;
            if (!PlayerInput.actions["Jump"].IsPressed())
            {
                // 消除跳跃力
                if (CanStopJump)
                    Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0);
            }

            if (Rigidbody.velocity.y <= 0)
            {
                IsJumping = false;
                GoToFall();
                return;
            }

            HandleMove();
        }

        private void GoToFall()
        {
            Animator.Play(FallHash);
            CurrentAction = FallUpdate;
        }

        private void FallUpdate()
        {
            UpdateAttack();

            if (IsOnGround)
            {
                GoToIdle();
                return;
            }

            HandleMove();
            Rigidbody.velocity = new Vector2(
                Rigidbody.velocity.x,
                Mathf.Max(Rigidbody.velocity.y, minFallSpeed)
            );
        }

        private void GoToHit()
        {
            Rigidbody.gravityScale = 1;
            Animator.Play(HitHash);
            CurrentAction = HitUpdate;
        }

        private void HitUpdate()
        {
            Rigidbody.velocity = HitVelocity;
        }
    }
}
