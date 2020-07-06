using OneVsMany;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Shared;
using System.Diagnostics;

namespace MissileDefense
{
    public class MissileCollisionSystem : SystemBase
    {
        EndSimulationEntityCommandBufferSystem endSimCommandBufferSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            endSimCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> buildings = EntityManager.CreateEntityQuery(
                typeof(Building),
                typeof(HealthInt),
                typeof(Radius))
                .ToEntityArray(Allocator.TempJob);

            NativeArray<Entity> defenses = EntityManager.CreateEntityQuery(
                typeof(Defense),
                typeof(Radius))
                .ToEntityArray(Allocator.TempJob);

            Entity score = EntityManager.CreateEntityQuery(typeof(Score)).GetSingletonEntity();
            //EntityCommandBuffer.Concurrent cmdBuffer = endSimCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            //EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(new EntityQueryDesc { a }));
            Entities                
                .WithReadOnly(buildings)
                .WithReadOnly(defenses)
                .WithDeallocateOnJobCompletion(buildings)
                .WithDeallocateOnJobCompletion(defenses)                
                .WithAll<Missile>()
                .ForEach((Entity missile, int entityInQueryIndex, ref Direction dir, ref DeletionMark mark,
                    in Translation translation, in Radius radius, in Damage damage) =>
            {
                for (int i = 0; i < buildings.Length; i++)
                {
                    Radius buildingRadius = GetComponentDataFromEntity<Radius>(true)[buildings[i]];
                    Translation buildingPos = GetComponentDataFromEntity<Translation>(true)[buildings[i]];

                    if (math.distance(translation.Value, buildingPos.Value) <= radius.value + buildingRadius.value)
                    {
                        // hit the building, reduce health
                        HealthInt health = GetComponentDataFromEntity<HealthInt>(false)[buildings[i]];
                        health.curr -= (int)damage.value;
                        SetComponent<HealthInt>(buildings[i], health);                        
                        mark.value = 1;
                    }
                }
                
                for (int i = 0; i < defenses.Length; i++)
                {
                    //Radius defenseRadius = GetComponentDataFromEntity<Radius>(true)[defenses[i]];
                    NonUniformScale defenseRadius = GetComponentDataFromEntity<NonUniformScale>(true)[defenses[i]];
                    Translation defensePos = GetComponentDataFromEntity<Translation>(true)[defenses[i]];
                    if (math.distance(translation.Value, defensePos.Value) <= radius.value + defenseRadius.Value.x * 0.25f)
                    {
                        mark.value = 1;
                        Score s = GetComponent<Score>(score);
                        s.value += 1;
                        SetComponent<Score>(score, s);
                    }
                }

                if (translation.Value.y < -3.5f)
                {
                    // below the ground
                    mark.value = 1;
                }
                

            }).Schedule();
        }
    }
}