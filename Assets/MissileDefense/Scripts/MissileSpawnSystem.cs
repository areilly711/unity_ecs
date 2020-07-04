using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Shared;
using System.Numerics;

namespace MissileDefense
{
    public class MissileSpawnSystem : SystemBase
    {
        float m_spawnTimer;
        Random m_random;
        float3 forward;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_random = new Random();
            m_random.InitState();
            forward = new float3(0, 1, 0);            
        }

        protected override void OnUpdate()
        {
            EntityQuery query = EntityManager.CreateEntityQuery(typeof(GameSettings));
            if (query.CalculateEntityCount() == 0) return;
            
            GameSettings settings = EntityManager.CreateEntityQuery(typeof(GameSettings)).GetSingleton<GameSettings>();

            if (m_spawnTimer >= settings.spawnRate)
            {
                EntityQuery buildingQuery = EntityManager.CreateEntityQuery(typeof(Building), typeof(Translation));
                if (buildingQuery.CalculateEntityCount() == 0)
                {
                    // all buildings have been destroyed, game over
                    return;
                }

                NativeArray<Translation> buildingPositions = buildingQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
                
                for (int i = 0; i < settings.spawns; i++)
                {
                    Entity missile = EntityManager.Instantiate(GamePrefabsAuthoring.Missile);
                    
                    // randomize x pos
                    Translation pos = EntityManager.GetComponentData<Translation>(missile);
                    pos.Value.x = m_random.NextFloat(settings.posMin, settings.posMax);
                    pos.Value.y = 5;
                    pos.Value.z = 0;
                    EntityManager.SetComponentData<Translation>(missile, pos);

                    // randomize speed
                    Speed s = EntityManager.GetComponentData<Speed>(missile);
                    s.value = m_random.NextFloat(settings.speedMin, settings.speedMax);
                    EntityManager.SetComponentData<Speed>(missile, s);

                    Rotation rot = EntityManager.GetComponentData<Rotation>(missile);
                    Translation target = buildingPositions[m_random.NextInt(buildingPositions.Length)];
                    float3 dir = math.normalize(target.Value - pos.Value);
                    EntityManager.SetComponentData<Direction>(missile, new Direction { value = dir });
                    float z = math.degrees(math.atan2(dir.y, dir.x));
                    rot.Value = quaternion.Euler(0, 0, z);                    
                    //rot.Value = quaternion.EulerZYX(0, 0, z);
                    //rot.Value = quaternion.LookRotationSafe(target.Value - pos.Value, forward);*/
                    //EntityManager.SetComponentData<Rotation>(missile, rot);


                    LocalToWorld ltw = EntityManager.GetComponentData<LocalToWorld>(missile);
                    ltw.Value = float4x4.EulerXYZ(0, 0, z);
                    //EntityManager.SetComponentData<LocalToWorld>(missile, ltw);

                }
                m_spawnTimer = 0;
                buildingPositions.Dispose();
            }
            m_spawnTimer += Time.DeltaTime;
            
            //EntityManager.Instantiate();
            /*Entities
                .WithReadOnly(buildingPositions)
                .WithDeallocateOnJobCompletion(buildingPositions)
                .ForEach((ref Translation translation, in Rotation rotation) =>
                {

            }).Schedule();*/
        }
    }
}