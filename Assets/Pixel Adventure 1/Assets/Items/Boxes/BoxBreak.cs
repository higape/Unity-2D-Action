using UnityEngine;

namespace MyGame
{
    public class BoxBreak : MonoBehaviour
    {
        // animation event
        private void OnBreakEnd()
        {
            Destroy(gameObject);
        }
    }
}
