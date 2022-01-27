/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Shared;

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
            // find all of the buildings
            NativeArray<Entity> buildings = EntityManager.CreateEntityQuery(
                typeof(Building),
                typeof(HealthInt),
                typeof(Radius))
                .ToEntityArray(Allocator.TempJob);

            if (buildings.Length == 0)
            {
                buildings.Dispose();
                return;
            }

            // find all the defenses
            NativeArray<Entity> defenses = EntityManager.CreateEntityQuery(
                typeof(Defense),
                typeof(Radius))
                .ToEntityArray(Allocator.TempJob);

            Entity score = EntityManager.CreateEntityQuery(typeof(Score)).GetSingletonEntity();
            
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
                    NonUniformScale defenseRadius = GetComponentDataFromEntity<NonUniformScale>(true)[defenses[i]];
                    Translation defensePos = GetComponentDataFromEntity<Translation>(true)[defenses[i]];
                    if (math.distance(translation.Value, defensePos.Value) <= radius.value + defenseRadius.Value.x * 0.25f)
                    {
                        // missile is overlapping the defense, mark the missile as destroyed and the player scores a point
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