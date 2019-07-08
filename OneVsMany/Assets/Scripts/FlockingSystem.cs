using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

namespace OneVsMany
{
    public class FlockingSystem : JobComponentSystem
    {
        float MaxForce = 0.03f;
        float DesiredSeparation = 0.5f;
        float NeighborDistance = 1f;

        [BurstCompile]
        [RequireComponentTag(typeof(Enemy))]
        struct DetermineMovementDirectionJob : IJobForEachWithEntity<Movement, Translation, BoundingVolume>
        {
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Translation> boidPositions;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Movement> boidVelocities;

            public Translation targetPosition;
            public float maxForce;
            public float desiredSeparation;
            public float neighborDistance;
            public float dt;

            public void Execute(Entity entity, int index, ref Movement movement, ref Translation position, ref BoundingVolume vol)
            {
                float3 accel = float3.zero;
                float3 seek = Utils.Seek(targetPosition.Value, position.Value, movement.speed);
                
                float3 sep = float3.zero;
                float3 align = float3.zero;
                float3 coh = float3.zero;
                int steerCount = 0;

                float3 alignSum = float3.zero;
                int alignCount = 0;

                float3 cohesionSum = float3.zero;
                int cohesionCount = 0;

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

                    // align
                    float3 boidVelocity = boidVelocities[i].direction;
                    if ((d > 0) && (d < neighborDistance))
                    {
                        alignSum += boidVelocity;
                        alignCount++;
                    }

                    // cohesion
                    if ((d > 0) && (d < neighborDistance))
                    {
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
                sep *= 2.5f;
                seek *= 0.05f;
                align *= 0.1f;
                coh *= 0.01f;

                accel += seek;
                accel += sep;
                accel += align;
                accel += coh;
                movement.direction += accel;
                movement.direction.z = 0;
                movement.direction = Utils.Clamp(movement.direction, movement.speed);

                position.Value += movement.direction * dt;
                vol.volume.center = position.Value;
                
                /*
                float3 sep = Utils.Separate(position.Value, movement.direction, boidPositions, desiredSeparation, movement.speed, maxForce);
                float3 align = Utils.Align(position.Value, movement.direction, boidPositions, boidVelocities, neighborDistance, movement.speed, maxForce);
                float3 coh = Utils.Cohesion(position.Value, movement.direction, boidPositions, neighborDistance, movement.speed);
                
                // add weights
                sep *= 2.5f;
                seek *= 0.05f;
                align *= 0.1f;
                coh *= 0.01f;

                accel += seek;
                accel += sep;
                accel += align;
                accel += coh;
                movement.direction += accel;
                movement.direction.z = 0;
                movement.direction = Utils.Clamp(movement.direction, movement.speed);

                position.Value += movement.direction * dt;
                vol.volume.center = position.Value;
                */
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityQuery boidQuery = EntityManager.CreateEntityQuery(typeof(Enemy), typeof(Movement), typeof(Translation));

            DetermineMovementDirectionJob job = new DetermineMovementDirectionJob()
            {
                boidPositions = boidQuery.ToComponentDataArray<Translation>(Allocator.TempJob),
                boidVelocities = boidQuery.ToComponentDataArray<Movement>(Allocator.TempJob),
                targetPosition = EntityManager.GetComponentData<Translation>(GameHandler.playerEntity),
                maxForce = MaxForce,
                desiredSeparation = DesiredSeparation,
                neighborDistance = NeighborDistance,
                dt = Time.deltaTime,
            };

            JobHandle jobHandle = job.Schedule(this, inputDeps);
            return jobHandle;
        }
    }
}
