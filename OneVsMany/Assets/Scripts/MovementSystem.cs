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
    [UpdateAfter(typeof(PlayerInputSystem))]
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
                return;
                //movement.direction = math.normalizesafe(targetPosition - position.Value);
                //position.Value += movement.direction * movement.speed * deltaTime;
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
                //if (bullet.isActive)
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
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            MoveTowardTargetJob job = new MoveTowardTargetJob()
            {
                targetPosition = EntityManager.GetComponentData<Translation>(GameHandler.playerEntity).Value,
                deltaTime = Time.deltaTime
            };

            JobHandle jobHandle = job.Schedule(this, inputDeps);

            BulletUpdateJob bulletJob = new BulletUpdateJob()
            {
                deltaTime = Time.deltaTime
            };
            jobHandle = bulletJob.Schedule(this, jobHandle);
            return jobHandle;
        }
    }
}
