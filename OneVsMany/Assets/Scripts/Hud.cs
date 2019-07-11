using UnityEngine;
using UnityEngine.UI;

namespace OneVsMany
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] GameHandler game;
        [SerializeField] Slider healthBar;
        [SerializeField] Text scoreText;
        [SerializeField] GameObject gameOverPanel;

        private void Start()
        {
            gameOverPanel.SetActive(false);
        }

        public void SetMaxHealth(float maxHealth)
        {
            healthBar.maxValue = maxHealth;
        }

        public void SetHealth(float health)
        {
            healthBar.value = health;
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }

        public void ShowGameOver()
        {
            game.GameOver();
            gameOverPanel.SetActive(true);
        }

        public void Restart()
        {
            game.Restart();
        }
    }
}
