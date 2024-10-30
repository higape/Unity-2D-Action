using UnityEngine;

namespace MyGame
{
    public class VerticalWindArea : WindAreaBase
    {
        [Tooltip("物体离开范围后其速度的缩放")]
        [Min(0)]
        public float forceScale = 0.8f;

        protected override void SetForce(float force, Collider2D other)
        {
            if (other.TryGetComponent<Character>(out var c))
            {
                // 向对象施加力
                c.Rigidbody.AddForce(new Vector2(0, force * Time.deltaTime), ForceMode2D.Impulse);
                // 阻止跳跃中断，因为物体突然停在半空会显得很突兀
                if (force > 0)
                {
                    c.CanStopJump = false;
                }
            }
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if (
                forceScale != 1
                && Common.ContainLayer(colliderMask.value, other.gameObject.layer)
                && other.TryGetComponent<Character>(out var c)
            )
            {
                // 调节离开范围的对象的垂直速度
                c.Rigidbody.velocity = new Vector2(
                    c.Rigidbody.velocity.x,
                    c.Rigidbody.velocity.y * forceScale
                );
            }
        }
    }
}
