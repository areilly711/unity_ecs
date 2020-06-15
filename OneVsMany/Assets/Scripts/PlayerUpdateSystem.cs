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
        float healthDegenRate = 1;
        Hud hud;

        public void Init(float playerDegenRate, Hud hud)
        {
            this.healthDegenRate = playerDegenRate;
            this.hud = hud;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // take input and move the player
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            float dt = World.Time.DeltaTime;
            float degenRate = healthDegenRate;

            //JobHandle jobHandle = playerMoverJob.Schedule(this, inputDeps);
            JobHandle jobHandle = Entities
                .WithAll<Player>()
                .ForEach((Entity entity, int entityInQueryIndex, ref Movement movement, 
                ref Translation position, ref BoundingVolume vol, ref Health health) =>
            {
                movement.direction.x = h;
                movement.direction.y = v;
                movement.direction = math.normalizesafe(new float3(h, v, 0));
                movement.direction.z = 0;

                position.Value += movement.direction * movement.speed * dt;
                vol.volume.center = position.Value;

                // decrease health
                health.curr -= dt * degenRate;
            }).Schedule(inputDeps);

            if (Input.GetMouseButtonDown(0)) // left click
            {
                // bullet was fired, finish the player job first
                jobHandle.Complete();
                float3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Translation playerPosition = GetComponentDataFromEntity<Translation>(true)[GameHandler.playerEntity];
                
                bool foundBullet = false;

               /* jobHandle =*/ Entities                    
                    .ForEach((Entity entity, int entityInQueryIndex, ref Bullet bullet, ref Movement movement, ref Translation position, ref BoundingVolume vol) =>
                {                    
                    if (!foundBullet && !bullet.isActive)
                    {
                        bullet.isActive = true;
                        vol.volume.center = position.Value = playerPosition.Value;
                        //scale.Value = 0.25f;
                        //vol.volume.extents.Set(scale.Value * 0.5f, scale.Value * 0.5f, scale.Value * 0.5f);
                        movement.speed = 7;
                        movement.direction = math.normalizesafe(clickPos - playerPosition.Value);
                        movement.direction.z = 0;

                        foundBullet = true;
                    }
                }).Run();
                
                //jobHandle = fireBulletJob.Schedule(this, jobHandle);
            }

            jobHandle.Complete();

            // update ui elements based on player data
            Player player = GetComponentDataFromEntity<Player>(true)[GameHandler.playerEntity];
            hud.SetScore(player.score);

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
