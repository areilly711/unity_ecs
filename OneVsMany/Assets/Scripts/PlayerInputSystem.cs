﻿using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

namespace OneVsMany
{
    public class PlayerInputSystem : ComponentSystem
    {
        float healthDegenRate = 1;

        protected override void OnUpdate()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Entities.WithAll<Player>().ForEach<Movement, Translation, BoundingVolume>((ref Movement movement, ref Translation position, ref BoundingVolume vol) =>
            {
                movement.direction.x = h;
                movement.direction.y = v;
                movement.direction = math.normalizesafe(new float3(h, v, 0));
                movement.direction.z = 0;

                position.Value += movement.direction * movement.speed * Time.deltaTime;
                vol.volume.center = position.Value;
            });


            Translation playerPos = GetComponentDataFromEntity<Translation>(true)[GameHandler.playerEntity];
            if (Input.GetMouseButtonDown(0)) // left click
            {
                // fire bullet
                float3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                EntityQuery bulletQuery = EntityManager.CreateEntityQuery(typeof(Bullet));
                NativeArray<Entity> bullets = bulletQuery.ToEntityArray(Allocator.TempJob);
                for (int i = 0; i < bullets.Length; i++)
                {
                    Entity e = bullets[i];
                    Scale scale = EntityManager.GetComponentData<Scale>(e);
                    BoundingVolume vol = EntityManager.GetComponentData<BoundingVolume>(e);
                    Translation position = EntityManager.GetComponentData<Translation>(e);
                    Movement movement = EntityManager.GetComponentData<Movement>(e);
                    Bullet bullet = EntityManager.GetComponentData<Bullet>(e);
                    if (!bullet.isActive)
                    {
                        bullet.isActive = true;
                        vol.volume.center = position.Value = playerPos.Value;
                        //scale.Value = 0.25f;
                        //vol.volume.extents.Set(scale.Value * 0.5f, scale.Value * 0.5f, scale.Value * 0.5f);
                        movement.speed = 7;
                        movement.direction = math.normalizesafe(clickPos - playerPos.Value);
                        movement.direction.z = 0;

                        EntityManager.SetComponentData<Scale>(e, scale);
                        EntityManager.SetComponentData<BoundingVolume>(e, vol);
                        EntityManager.SetComponentData<Translation>(e, position);
                        EntityManager.SetComponentData<Movement>(e, movement);
                        EntityManager.SetComponentData<Bullet>(e, bullet);
                        break;
                    }
                }

                bullets.Dispose();
            }

            Entities.WithAll<Player>().ForEach<Health>((ref Health health) =>
            {
                health.curr -= Time.deltaTime * healthDegenRate;
            });
        }
    }
    
    /*
    public class PlayerMovementSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            return inputDeps;
        }
    }
    */
}
