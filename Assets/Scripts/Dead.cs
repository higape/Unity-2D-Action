using UnityEngine;

namespace MyGame
{
    public class Dead : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<Character>(out var character))
            {
                character.Dead();
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.TryGetComponent<Character>(out var character))
            {
                character.Dead();
            }
        }
    }
}
