using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
using UnityEditor;
using Shared;
using Unity.Mathematics;

namespace MissileDefense
{
    public class GameHandler : MonoBehaviour
    {
        public Image m_reloadBar;
        public TextMeshProUGUI m_score;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
            GameSettings gameSettings = em.CreateEntityQuery(typeof(GameSettings)).GetSingleton<GameSettings>();
            AttackSpeed playerSettings = em.CreateEntityQuery(typeof(Player), typeof(AttackSpeed)).GetSingleton<AttackSpeed>();
            Score score = em.CreateEntityQuery(typeof(Score)).GetSingleton<Score>();
            m_reloadBar.fillAmount = math.lerp(0, 1, 1 - (float)playerSettings.counter / (float)playerSettings.speed);
            m_score.text = score.value.ToString();

        }
    }
}
