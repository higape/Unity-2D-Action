using UnityEngine;

namespace MyGame
{
    public class Box : MonoBehaviour, IHit
    {
        public int hp = 1;
        public GameObject breakPrefab;

        private Animator Animator { get; set; }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public void Hit(int attack, Vector2 direction)
        {
            if (hp > 0)
            {
                hp -= attack;
                if (hp <= 0)
                {
                    Instantiate(breakPrefab, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                else
                {
                    Animator.Play(Character.HitHash);
                }
            }
        }
    }
}
