using System;
using System.Linq;
using UnityEngine;

namespace MyGame
{
    public sealed class Bee : Character
    {
        public Vector2 attackRange;
        public float blankTime = 1.5f;
        public GameObject bulletPrefab;
        public Vector3 bulletPosition;
        public Vector2 bulletVelocity;

        private Action CurrentAction { get; set; }
        private Action TempAction { get; set; }
        private float StateTimeCount { get; set; }

        private void Start()
        {
            GoToIdle();
        }

        private void Update()
        {
            UpdateInvalidTime();
            Rigidbody.velocity = new Vector2(0, 0);
            if (StateTimeCount > 0)
                StateTimeCount -= Time.deltaTime;
            CurrentAction?.Invoke();
        }

        private void DelayOneFrame(Action action)
        {
            TempAction = action;
            CurrentAction = () => CurrentAction = TempAction;
        }

        private void GoToIdle()
        {
            Animator.Play(IdleHash);
            CurrentAction = IdleUpdate;
        }

        private void IdleUpdate()
        {
            // 如果已准备好攻击
            if (StateTimeCount <= 0)
            {
                // 如果攻击范围内存在玩家
                if (CheckRangeDown(attackRange, PlayerLayer, PlayerLayer | AllGround))
                {
                    // 开始攻击
                    GoToAttack();
                }
            }
        }

        private void GoToAttack()
        {
            Animator.Play(AttackHash);
            CurrentAction = null;
        }

        // animation event
        private void OnAttackEnd()
        {
            DelayOneFrame(GoToIdle);
        }

        // animation event
        private void OnBulletLaunch()
        {
            // 创建子弹
            var bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
            bullet.transform.position = transform.position + bulletPosition;
            bullet.Setup(
                BulletCallback,
                PlayerLayer | AllGround,
                bulletVelocity,
                Bullet.DefaultDeadTime
            );
            StateTimeCount = blankTime;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (UnityEditor.Selection.gameObjects.Contains(gameObject))
            {
                Gizmos.color = Color.red;
                if (TryGetComponent<BoxCollider2D>(out var collider))
                {
                    GizmosDrawRangeDown(collider, attackRange);
                }
                Gizmos.DrawSphere(transform.position + bulletPosition, 0.03125f);
            }
        }
#endif
    }
}
