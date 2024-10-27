using UnityEngine;

namespace MyGame
{
    public class Checkpoint : MonoBehaviour
    {
        private static readonly Vector2 Offset = new(0, -0.5f);
        private bool IsFaceRight => transform.rotation.eulerAngles.y == 0;
        private Animator Animator { get; set; }
        private bool IsOut { get; set; }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsOut && Common.ContainLayer(Character.PlayerLayer, other.gameObject.layer))
            {
                IsOut = true;
                Animator.Play("Out");
                Player.RebornPosition = (Vector2)transform.position + Offset;
                Player.RebornFaceRight = IsFaceRight;
            }
        }
    }
}
