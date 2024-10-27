using TMPro;
using UnityEngine;

namespace MyGame
{
    public class SceneUI : MonoBehaviour
    {
        private static SceneUI Instance { get; set; }

        public static void RefreshLife(int value)
        {
            if (Instance != null)
                Instance.hpText.text = value.ToString();
        }

        public static void RefreshScore(int value)
        {
            if (Instance != null)
                Instance.scoreText.text = value.ToString();
        }

        public TextMeshProUGUI hpText;
        public TextMeshProUGUI scoreText;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(this, Instance))
                Instance = null;
        }
    }
}
