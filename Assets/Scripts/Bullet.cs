using System;
using UnityEngine;

namespace MyGame
{
    public class Bullet : MonoBehaviour
    {
        public const float DefaultDeadTime = 2f;
        private Animator Animator { get; set; }
        private Rigidbody2D Rigidbody { get; set; }
        private Action<Collider2D, Vector2> Callback { get; set; }
        private LayerMask LayerMask { get; set; }
        private float LifeTime { get; set; }
        private float TimeCount { get; set; }
        private bool IsAlreadyHit { get; set; }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        protected void Update()
        {
            TimeCount += Time.deltaTime;
            if (TimeCount >= LifeTime)
            {
                DestroyGameObject();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsAlreadyHit && Common.ContainLayer(LayerMask.value, other.gameObject.layer))
            {
                IsAlreadyHit = true;
                Rigidbody.velocity = Vector2.zero;
                Animator.Play(Character.HitHash);
                Callback?.Invoke(other, other.transform.position - transform.position);
            }
        }

        public void Setup(
            Action<Collider2D, Vector2> callback,
            LayerMask layerMask,
            Vector2 velocity,
            float lifeTime
        )
        {
            Callback = callback;
            LayerMask = layerMask;
            Rigidbody.velocity = velocity;
            LifeTime = lifeTime;
        }

        // animation event
        private void OnHitEnd() => DestroyGameObject();

        private void DestroyGameObject() => Destroy(gameObject);
    }
}
