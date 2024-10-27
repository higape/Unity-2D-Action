using UnityEngine;

namespace MyGame
{
    public class HorizontalWindArea : WindAreaBase
    {
        // 向对象施加力
        protected override void SetForce(float force, Collider2D other)
        {
            if (other.TryGetComponent<Character>(out var c))
            {
                c.AddedVelocityX += force;
            }
        }
    }
}
