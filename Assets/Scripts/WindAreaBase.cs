using UnityEngine;

namespace MyGame
{
    public abstract class WindAreaBase : MonoBehaviour
    {
        [Tooltip("碰撞体遮罩。只对指定层的碰撞体施加作用。")]
        public LayerMask colliderMask = new() { value = -1 };

        [Tooltip("力的大小和方向。")]
        public float force = 2;

        [Tooltip("每单位距离力增量。力的方向不会因为增量而改变。")]
        public float forceIncrement = 0;

        protected void OnTriggerStay2D(Collider2D other)
        {
            // 检查对象的层
            if (Common.ContainLayer(colliderMask.value, other.gameObject.layer))
            {
                // 计算风力，并向对象施加力
                SetForce(CalculateForce(other), other);
            }
        }

        // 向对象施加力
        protected abstract void SetForce(float force, Collider2D other);

        // 根据距离计算风力
        protected float CalculateForce(Collider2D other)
        {
            if (forceIncrement != 0)
            {
                // 计算对象距离
                float distance = Vector2.Distance(transform.position, other.transform.position);
                // 计算风力
                float windForce = force + distance * forceIncrement;
                // 防止反方向的力
                if ((windForce > 0 && force >= 0) || (windForce < 0 && force <= 0))
                    return windForce;
                else
                    return 0;
            }
            return force;
        }
    }
}
