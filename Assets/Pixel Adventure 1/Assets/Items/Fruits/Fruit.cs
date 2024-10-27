using UnityEngine;

namespace MyGame
{
    public class Fruit : MonoBehaviour
    {
        public static int CollectedHash = Animator.StringToHash("Collected");

        private Animator Animator { get; set; }
        private bool IsCollected { get; set; }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsCollected && Common.ContainLayer(Character.PlayerLayer, other.gameObject.layer))
            {
                IsCollected = true;
                Animator.Play(CollectedHash);
                Player.AddFruit();
            }
        }

        // animation event
        private void OnCollectedEnd()
        {
            Destroy(gameObject);
        }
    }
}
