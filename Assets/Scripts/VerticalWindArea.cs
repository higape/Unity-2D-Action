using UnityEngine;

namespace MyGame
{
    public class VerticalWindArea : WindAreaBase
    {
        [Tooltip("物体离开范围后其速度的缩放")]
        [Min(0)]
        public float forceScale = 0.8f;

        // 向对象施加力
        protected override void SetForce(float force, Collider2D other)
        {
            other.attachedRigidbody.AddForce(
                new Vector2(0, force * Time.deltaTime),
                ForceMode2D.Impulse
            );

            // 阻止跳跃中断，因为物体突然停在半空会显得很突兀
            if (other.TryGetComponent<Character>(out var c))
            {
                c.CanStopJump = false;
            }
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (forceScale != 1)
            {
                // 调节离开范围的对象的垂直速度
                other.attachedRigidbody.velocity = new Vector2(
                    other.attachedRigidbody.velocity.x,
                    other.attachedRigidbody.velocity.y * forceScale
                );
            }
        }
    }
}
