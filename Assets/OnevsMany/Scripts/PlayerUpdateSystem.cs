/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Shared;

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

            JobHandle jobHandle = Entities
                .WithAll<Player>()
                .ForEach((Entity entity, int entityInQueryIndex, ref Movement movement, 
                ref Translation position, ref BoundingVolume vol, ref HealthFloat health) =>
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

               Entities.ForEach((Entity entity, int entityInQueryIndex, ref Bullet bullet,
                   ref Movement movement, ref Translation position, ref BoundingVolume vol) =>
                {                    
                    if (!foundBullet && !bullet.isActive)
                    {
                        bullet.isActive = true;
                        vol.volume.center = position.Value = playerPosition.Value;
                        movement.speed = 7;
                        movement.direction = math.normalizesafe(clickPos - playerPosition.Value);
                        movement.direction.z = 0;

                        foundBullet = true;
                    }
                }).Run();
                
            }

            jobHandle.Complete();

            // update ui elements based on player data
            Player player = GetComponentDataFromEntity<Player>(true)[GameHandler.playerEntity];
            hud.SetScore(player.score);

            HealthFloat playerHealth = GetComponentDataFromEntity<HealthFloat>(true)[GameHandler.playerEntity];
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
