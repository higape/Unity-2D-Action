using UnityEngine;

namespace MyGame
{
    public class CheckpointEnd : MonoBehaviour
    {
        private Animator Animator { get; set; }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Common.ContainLayer(Character.PlayerLayer, other.gameObject.layer))
            {
                Animator.Play("Pressed");
            }
        }
    }
}
