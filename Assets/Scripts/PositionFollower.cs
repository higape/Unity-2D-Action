using UnityEngine;

namespace MyGame
{
    public class PositionFollower : MonoBehaviour
    {
        public Transform target;
        public Vector2 positionOffset;
        public Vector2 minPosition;
        public Vector2 maxPosition;

        private void LateUpdate()
        {
            if (target != null)
            {
                transform.position = new(
                    positionOffset.x + Mathf.Clamp(target.position.x, minPosition.x, maxPosition.x),
                    positionOffset.y + Mathf.Clamp(target.position.y, minPosition.y, maxPosition.y),
                    transform.position.z
                );
            }
        }
    }
}
