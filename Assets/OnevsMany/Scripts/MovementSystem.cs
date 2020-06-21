using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;

namespace OneVsMany
{
    [UpdateAfter(typeof(FlockingSystem))]
    [UpdateAfter(typeof(PlayerUpdateSystem))]
    public class MovementSystem : JobComponentSystem
    {
        /// <summary>
        /// Updates active bullet positions and bounding volumes
        /// </summary>
        /*[BurstCompile]
        struct BulletUpdateJob : IJobForEach<Bullet, Movement, Translation, BoundingVolume>
        {
            public float deltaTime;

            public void Execute(ref Bullet bullet, ref Movement movement, ref Translation position, ref BoundingVolume vol)
            {
                if (bullet.isActive)
                {
                    // move the bullet along the direction that it was shot
                    position.Value += movement.direction * movement.speed * deltaTime;
                    vol.volume.center = position.Value;
                    bullet.age += deltaTime;

                    if (bullet.age >= 3)
                    {
                        bullet.isActive = false;
                        bullet.age = 0;
                    }
                }
                else
                {
                    position.Value.x = 1000;
                    movement.direction = float3.zero;
                }
            }
        }*/

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {            
            float dt = World.Time.DeltaTime;

            JobHandle jobHandle = Entities.ForEach((ref Bullet bullet, ref Movement movement, ref Translation position, ref BoundingVolume vol) =>
            {
                if (bullet.isActive)
                {
                    // move the bullet along the direction that it was shot
                    position.Value += movement.direction * movement.speed * dt;
                    vol.volume.center = position.Value;
                    bullet.age += dt;

                    if (bullet.age >= 3)
                    {
                        bullet.isActive = false;
                        bullet.age = 0;
                    }
                }
                else
                {
                    position.Value.x = 1000;
                    movement.direction = float3.zero;
                }
            }).Schedule(inputDeps);
            jobHandle.Complete();
            return jobHandle;
        }
    }
}
