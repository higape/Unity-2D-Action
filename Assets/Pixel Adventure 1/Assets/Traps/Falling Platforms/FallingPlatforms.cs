using UnityEngine;

namespace MyGame
{
    public class FallingPlatforms : MonoBehaviour
    {
        public float fallSpeed = -2;
        private Rigidbody2D Rigidbody { get; set; }
        private bool IsFalling { get; set; }
        private ContactPoint2D[] ContactPoints { get; set; } = new ContactPoint2D[2];

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsFalling)
            {
                int count = collision.GetContacts(ContactPoints);
                if (count >= 2)
                {
                    if (
                        collision.transform.position.y > transform.position.y
                        && Mathf.Abs(ContactPoints[0].point.y - ContactPoints[1].point.y)
                            < Character.CastDifference
                    )
                    {
                        Rigidbody.velocity = new(0, fallSpeed);
                        IsFalling = true;
                    }
                }
            }
        }
    }
}
