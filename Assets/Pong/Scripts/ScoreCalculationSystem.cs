using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Shared;

namespace Pong
{
    public class ScoreCalculationSystem : SystemBase
    {
        GameHandler m_handler;
        protected override void OnCreate()
        {
            base.OnCreate();
            m_handler = UnityEngine.GameObject.FindObjectOfType<GameHandler>();
        }

        protected override void OnUpdate()
        {
            // get the ball's position
            EntityQuery query = EntityManager.CreateEntityQuery(typeof(Translation), typeof(Ball));
            if (query.CalculateEntityCount() != 1)
            {
                return;
            }
            float3 ballPos = EntityManager.CreateEntityQuery(typeof(Translation), typeof(Ball)).GetSingleton<Translation>().Value;

            query = EntityManager.CreateEntityQuery(typeof(Translation), typeof(Player));
            if (query.CalculateEntityCount() != 1)
            {
                return;
            }
            float3 playerPos = query.GetSingleton<Translation>().Value;

            query = EntityManager.CreateEntityQuery(typeof(Translation), typeof(AI));
            if (query.CalculateEntityCount() != 1)
            {
                return;
            }
            float3 aiPos = query.GetSingleton<Translation>().Value;

            if (ballPos.x <= playerPos.x)
            {
                // ai scored
                m_handler.PointEarned(1);
            }
            else if (ballPos.x >= aiPos.x)
            {
                // player scored
                m_handler.PointEarned(0);
            }
        }
    }
}