using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneVsMany
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] Slider healthBar;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void SetMaxHealth(float maxHealth)
        {
            healthBar.maxValue = maxHealth;
        }

        public void SetHealth(float health)
        {
            healthBar.value = health;
        }
    }
}
