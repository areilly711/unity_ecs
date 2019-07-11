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
        [BurstCompile]
        [ExcludeComponent(typeof(Player), typeof(Bullet), typeof(Enemy))]
        struct MoveTowardTargetJob : IJobForEach<Movement, Translation, BoundingVolume>
        {
            public float3 targetPosition;
            public float deltaTime;

            public void Execute(ref Movement movement, ref Translation position, ref BoundingVolume vol)
            {
                position.Value += movement.direction * deltaTime;
                vol.volume.center = position.Value;
            }
        }

        [BurstCompile]
        struct BulletUpdateJob : IJobForEach<Bullet, Movement, Translation, BoundingVolume>
        {
            public float deltaTime;

            public void Execute(ref Bullet bullet, ref Movement movement, ref Translation position, ref BoundingVolume vol)
            {
                if (bullet.isActive)
                {
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
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            BulletUpdateJob bulletJob = new BulletUpdateJob()
            {
                deltaTime = Time.deltaTime
            };
            JobHandle jobHandle = bulletJob.Schedule(this, inputDeps);
            return jobHandle;
        }
    }
}
