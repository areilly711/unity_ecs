/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Shared;

namespace OneVsMany
{
    public partial class CollisionSystem : JobComponentSystem
    {        
        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityCommandBuffer.Concurrent commandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            BoundingVolume playerBounds = GetComponentDataFromEntity<BoundingVolume>(true)[GameHandler.playerEntity];
            HealthFloat playerHealth = GetComponentDataFromEntity<HealthFloat>(true)[GameHandler.playerEntity];
            Player player = GetComponentDataFromEntity<Player>(true)[GameHandler.playerEntity];

            // job for collision between player and enemies
            Entity playerEntity = GameHandler.playerEntity;
            JobHandle jobHandle = Entities
                .WithNone<Bullet>()
                .ForEach((Entity entity, int entityInQueryIndex, ref BoundingVolume vol, ref HealthModifier healthMod) =>
            {
                if (vol.volume.Intersects(playerBounds.volume))
                {
                    // there was a collision, modify the player's health
                    Utils.ModifyHealth(ref playerHealth, healthMod.value);
                    commandBuffer.SetComponent<HealthFloat>(entityInQueryIndex, playerEntity, playerHealth);

                    // get rid of the damager
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                }
            }).Schedule(inputDeps);

            EntityQuery bulletQuery = EntityManager.CreateEntityQuery(typeof(Bullet), ComponentType.ReadOnly<BoundingVolume>(), ComponentType.ReadOnly<HealthModifier>());
            bulletQuery.AddDependency(jobHandle);
            
            NativeArray<BoundingVolume> bulletColliders = bulletQuery.ToComponentDataArray<BoundingVolume>(Allocator.TempJob);
            NativeArray<HealthModifier> bulletHealthMods = bulletQuery.ToComponentDataArray<HealthModifier>(Allocator.TempJob);
            NativeArray<Bullet> bulletInfos = bulletQuery.ToComponentDataArray<Bullet>(Allocator.TempJob);
            NativeArray<Entity> bullets = bulletQuery.ToEntityArray(Allocator.TempJob);

            // job for checking collisions between enemies and bullets
            jobHandle = Entities
                .WithDeallocateOnJobCompletion(bulletColliders)
                .WithDeallocateOnJobCompletion(bulletHealthMods)
                .WithDeallocateOnJobCompletion(bulletInfos)
                .WithDeallocateOnJobCompletion(bullets)
                .WithReadOnly(bulletColliders)
                .WithReadOnly(bulletHealthMods)
                .WithReadOnly(bulletInfos)
                .WithReadOnly(bullets)
                .WithNone<Player>()
                .WithAll<Enemy>()
                .ForEach((Entity entity, int entityInQueryIndex, ref BoundingVolume damageableCollider, ref HealthFloat damageableHealth) =>
            {
                for (int i = 0; i < bulletColliders.Length; i++)
                {
                    // bullet isn't active, leave
                    if (!bulletInfos[i].isActive) continue;

                    if (damageableCollider.volume.Intersects(bulletColliders[i].volume))
                    {
                        // bullet hit a damageable, reduce it's health
                        Utils.ModifyHealth(ref damageableHealth, bulletHealthMods[i].value);

                        // deactivate the bullet
                        Bullet b = bulletInfos[i];
                        b.isActive = false;
                        b.age = 0;
                        commandBuffer.SetComponent<Bullet>(entityInQueryIndex, bullets[i], b);
                    }
                }
            }).Schedule(jobHandle);
            jobHandle.Complete();

            jobHandle = Entities
                .ForEach((Entity entity, int entityInQueryIndex, ref Enemy enemy, ref HealthFloat  health) =>
            {
                
                if (health.curr <= 0)
                {
                    // destroy any entity who's health has dropped below 0
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                    player.score += enemy.points;
                    commandBuffer.SetComponent<Player>(entityInQueryIndex, playerEntity, player);
                }
            }).Schedule(jobHandle);
            jobHandle.Complete();

            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
            return jobHandle;
        }
    }
}
