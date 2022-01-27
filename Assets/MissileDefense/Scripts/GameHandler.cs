/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
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
