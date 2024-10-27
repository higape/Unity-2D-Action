using UnityEngine;

namespace MyGame
{
    public class CheckpointStart : MonoBehaviour
    {
        private static readonly Vector2 Offset = new(11f / 32f, -0.5f);

        private bool IsFaceRight => transform.rotation.eulerAngles.y == 0;

        private void Awake()
        {
            Player.RebornPosition = (Vector2)transform.position + Offset;
            Player.RebornFaceRight = IsFaceRight;
        }
    }
}
