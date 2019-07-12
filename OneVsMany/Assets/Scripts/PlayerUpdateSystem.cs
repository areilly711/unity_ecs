using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

namespace OneVsMany
{
    public class PlayerUpdateSystem : JobComponentSystem
    {
        [RequireComponentTag(typeof(Player))]
        struct PlayerUpdateJob : IJobForEachWithEntity<Movement, Translation, BoundingVolume, Health>
        {
            public float h;
            public float v;
            public float dt;
            public float healthDegenRate;

            public void Execute(Entity entity, int index, ref Movement movement, ref Translation position, ref BoundingVolume vol, ref Health health)
            {
                movement.direction.x = h;
                movement.direction.y = v;
                movement.direction = math.normalizesafe(new float3(h, v, 0));
                movement.direction.z = 0;

                position.Value += movement.direction * movement.speed * dt;
                vol.volume.center = position.Value;

                // decrease health
                health.curr -= dt * healthDegenRate;
            }
        }


        struct BulletFireJob : IJobForEachWithEntity<Bullet, Movement, Translation, BoundingVolume>
        {
            public float3 playerPos;
            public float3 clickPos;
            bool foundBullet;

            public void Execute(Entity entity, int index, ref Bullet bullet, ref Movement movement, ref Translation position, ref BoundingVolume vol)
            {
                if (foundBullet) return;

                if (!bullet.isActive)
                {
                    bullet.isActive = true;
                    vol.volume.center = position.Value = playerPos;
                    //scale.Value = 0.25f;
                    //vol.volume.extents.Set(scale.Value * 0.5f, scale.Value * 0.5f, scale.Value * 0.5f);
                    movement.speed = 7;
                    movement.direction = math.normalizesafe(clickPos - playerPos);
                    movement.direction.z = 0;

                    foundBullet = true;
                }
            }
        }

        float healthDegenRate = 1;
        Hud hud;

        public void Init(float playerDegenRate, Hud hud)
        {
            this.healthDegenRate = playerDegenRate;
            this.hud = hud;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            PlayerUpdateJob playerMoverJob = new PlayerUpdateJob()
            {
                h = h,
                v = v,
                dt = Time.deltaTime,
                healthDegenRate = healthDegenRate,
            };

            JobHandle jobHandle = playerMoverJob.Schedule(this, inputDeps);

            if (Input.GetMouseButtonDown(0))
            {
                jobHandle.Complete();
                float3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Translation playerPosition = GetComponentDataFromEntity<Translation>(true)[GameHandler.playerEntity];

                BulletFireJob fireBulletJob = new BulletFireJob()
                {
                    clickPos = clickPos,
                    playerPos = playerPosition.Value,
                };

                jobHandle = fireBulletJob.Schedule(this, jobHandle);
            }

            jobHandle.Complete();

            Player player = GetComponentDataFromEntity<Player>(true)[GameHandler.playerEntity];
            hud.SetScore(player.score);

            //EntityQuery isPlayerAliveQuery = EntityManager.CreateEntityQuery(typeof(Player), typeof(PlayerSystemState));
            Health playerHealth = GetComponentDataFromEntity<Health>(true)[GameHandler.playerEntity];
            hud.SetHealth(playerHealth.curr);

            if (playerHealth.curr <= 0)
            {
                // player died, end the game
                hud.ShowGameOver();
            }
            
            return jobHandle;
        }
    }
}
