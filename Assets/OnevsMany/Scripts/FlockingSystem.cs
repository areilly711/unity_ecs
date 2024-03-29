﻿/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

namespace OneVsMany
{
    public class FlockingSystem : JobComponentSystem
    {
        float MaxForce = 0.03f;
        float DesiredSeparation = 0.5f;
        float NeighborDistance = 1f;

        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // get all the flocking agents
            EntityQuery boidQuery = EntityManager.CreateEntityQuery(typeof(Enemy), typeof(Movement), typeof(Translation));
            Translation playerPos = EntityManager.GetComponentData<Translation>(GameHandler.playerEntity);            
            NativeArray<Translation> boidPositions = boidQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Movement> boidVelocities = boidQuery.ToComponentDataArray<Movement>(Allocator.TempJob);

            float dt = Time.DeltaTime;
            float desiredSeparation = DesiredSeparation;
            float maxForce = MaxForce;
            float neighborDistance = NeighborDistance;

            float sepWeight = 2.5f;
            float seekWeight = 0.05f;            
            float alignWeight = 0.1f;
            float cohesionWeight = 0.01f;

            //JobHandle jobHandle = job.Schedule(this, inputDeps);
            JobHandle jobHandle = Entities
                .WithDeallocateOnJobCompletion(boidPositions)
                .WithDeallocateOnJobCompletion(boidVelocities)
                .WithReadOnly(boidPositions)
                .WithReadOnly(boidVelocities)
                .WithAll<Enemy>()
                .ForEach((Entity entity, int entityInQueryIndex, ref Movement movement, ref Translation position, ref BoundingVolume vol) =>
                {
                    float3 accel = float3.zero;
                    float3 seek = Utils.Seek(playerPos.Value, position.Value, movement.speed);

                    float3 sep = float3.zero;
                    float3 align = float3.zero;
                    float3 coh = float3.zero;
                    int steerCount = 0;

                    float3 alignSum = float3.zero;
                    int alignCount = 0;

                    float3 cohesionSum = float3.zero;
                    int cohesionCount = 0;

                    // Craig Reynolds flocking method
                    // For every boid in the system, check if it's too close
                    for (int i = 0; i < boidPositions.Length; i++)
                    {
                        // separation 
                        Translation b = boidPositions[i];
                        float d = math.distance(position.Value, b.Value);

                        // If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
                        if ((d > 0) && (d < desiredSeparation))
                        {
                            // Calculate vector pointing away from neighbor
                            float3 diff = position.Value - b.Value;
                            diff = math.normalizesafe(diff);
                            diff /= d;        // Weight by distance
                            sep += diff;
                            steerCount++;            // Keep track of how many
                        }


                        float3 boidVelocity = boidVelocities[i].direction;
                        if ((d > 0) && (d < neighborDistance))
                        {
                            // align
                            alignSum += boidVelocity;
                            alignCount++;

                            // cohesion
                            cohesionSum += b.Value; // Add position
                            cohesionCount++;
                        }
                    }

                    // Average -- divide by how many
                    if (steerCount > 0)
                    {
                        sep /= steerCount;
                    }

                    // As long as the vector is greater than 0
                    if (math.length(sep) > 0)
                    {
                        // Implement Reynolds: Steering = Desired - Velocity
                        sep = math.normalizesafe(sep);
                        sep *= movement.speed;
                        sep -= movement.direction;
                        sep = Utils.Clamp(sep, maxForce);
                    }

                    // align
                    if (alignCount > 0)
                    {
                        alignSum /= alignCount;

                        // Implement Reynolds: Steering = Desired - Velocity
                        alignSum = math.normalizesafe(alignSum);
                        alignSum *= movement.speed;
                        align = alignSum - movement.direction;
                        align = Utils.Clamp(sep, maxForce);
                    }

                    if (cohesionCount > 0)
                    {
                        cohesionSum /= cohesionCount;
                        coh = Utils.Seek(cohesionSum, position.Value, movement.speed);  // Steer towards the position
                    }
                    else
                    {
                        coh = float3.zero;
                    }

                    // use weights
                    sep *= sepWeight;
                    seek *= seekWeight;
                    align *= alignWeight;
                    coh *= cohesionWeight;

                    accel += seek;
                    accel += sep;
                    accel += align;
                    accel += coh;
                    movement.direction += accel;
                    movement.direction.z = 0;
                    movement.direction = Utils.Clamp(movement.direction, movement.speed);

                    position.Value += movement.direction * dt;
                    vol.volume.center = position.Value;
                }).Schedule(inputDeps);

            jobHandle.Complete();

            jobHandle = Entities
                .WithAll<Enemy>()
                .ForEach((ref Translation position, ref NonUniformScale scale) =>
            {
                float dir = playerPos.Value.x - position.Value.x;
                scale.Value.x = scale.Value.y * math.sign(dir);
            }).Schedule(jobHandle);
            jobHandle.Complete();

            return jobHandle;
        }
    }
}
