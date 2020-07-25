using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
using UnityEditor;
using Shared;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

namespace MissileDefense
{
    /// <summary>
    /// Main entry point of the game. The only MonoBehaviour
    /// </summary>
    public class GameHandler : MonoBehaviour
    {
        public GameObject m_gameoverPanel;
        public Image m_reloadBar;
        public TextMeshProUGUI m_score;

        // Update is called once per frame
        void Update()
        {
            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;

            // get the game and player settings in order to update the ui
            GameSettings gameSettings = em.CreateEntityQuery(typeof(GameSettings)).GetSingleton<GameSettings>();
            AttackSpeed playerSettings = em.CreateEntityQuery(typeof(Player), typeof(AttackSpeed)).GetSingleton<AttackSpeed>();
            Score score = em.CreateEntityQuery(typeof(Score)).GetSingleton<Score>();
            m_reloadBar.fillAmount = math.lerp(0, 1, 1 - (float)playerSettings.counter / (float)playerSettings.speed);
            m_score.text = score.value.ToString();


            int buildingCount = em.CreateEntityQuery(
                typeof(Building),
                typeof(HealthInt),
                typeof(Radius))
                .CalculateEntityCount(); 
            

            // game over create show UI
            m_gameoverPanel.SetActive(buildingCount == 0);
        }

        public void ResetGame()
        {
            // find the entity building positions
            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
            NativeArray<Translation> buildingPositions = em.CreateEntityQuery(typeof(Translation), typeof(BuildingPositionMarker))
                .ToComponentDataArray<Translation>(Allocator.TempJob);

            // use the positions to create and position new buildings
            for (int i = 0; i < buildingPositions.Length; i++)
            {
                Entity e = em.Instantiate(GamePrefabsAuthoring.Building);
                em.SetComponentData<Translation>(e, buildingPositions[i]);
            }

            buildingPositions.Dispose();
        }
    }
}
