using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

namespace OneVsMany
{
    public partial class CollisionSystem : JobComponentSystem
    {
        /// <summary>
        /// Checks if the player is colliding with a health modifier
        /// </summary>
        [ExcludeComponent(typeof(Bullet))]
        //[BurstCompile]
        struct PlayerToHealthModifierCollisionJob : IJobForEachWithEntity<BoundingVolume, HealthModifier>
        {
            public Entity player;
            public Bounds playerBounds;
            public Health playerHealth;
            public EntityCommandBuffer.Concurrent commandBuffer;

            public void Execute(Entity entity, int index, ref BoundingVolume vol, ref HealthModifier healthMod)
            {
                if (vol.volume.Intersects(playerBounds))
                {
                    // there was a collision, modify the player's health
                    Utils.ModifyHealth(ref playerHealth, healthMod.value);
                    commandBuffer.SetComponent<Health>(index, player, playerHealth);

                    // get rid of the damager
                    commandBuffer.DestroyEntity(index, entity);
                }
            }
        }

        /// <summary>
        /// Detects if there is a collision between bullets and damageables. If a collision occurs,
        /// the bullet is deactivated and the health is reduced
        /// </summary>
        //[BurstCompile()]
        [ExcludeComponent(typeof(Player))]
        [RequireComponentTag(typeof(Enemy))]
        public struct BulletToDamageableCollisionJob : IJobForEachWithEntity<BoundingVolume, Health>
        {
            public EntityCommandBuffer.Concurrent commandBuffer;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<BoundingVolume> bulletColliders;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<HealthModifier> bulletHealthMods;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Bullet> bulletInfos;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> bullets;

            public void Execute(Entity entity, int index, ref BoundingVolume damageableCollider, ref Health damageableHealth)
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
                        commandBuffer.SetComponent<Bullet>(index, bullets[i], b);
                    }
                }
            }
        }

        /// <summary>
        /// Checks all Health components to see if their health is less than 0 and
        /// destroys the entity if that is the case
        /// </summary>
        //[BurstCompile()]
        struct ValidateLifeJob : IJobForEachWithEntity<Enemy, Health>
        {
            public Entity playerEntity;
            public Player player;
            public EntityCommandBuffer.Concurrent commandBuffer;
            public void Execute(Entity entity, int index, ref Enemy enemy, ref Health health)
            {
                if (health.curr <= 0)
                {
                    commandBuffer.DestroyEntity(index, entity);
                    player.score += enemy.points;
                    commandBuffer.SetComponent<Player>(index, playerEntity, player);
                }
            }
        }

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
            Health playerHealth = GetComponentDataFromEntity<Health>(true)[GameHandler.playerEntity];
            Player player = GetComponentDataFromEntity<Player>(true)[GameHandler.playerEntity];
            
            PlayerToHealthModifierCollisionJob pToHJob = new PlayerToHealthModifierCollisionJob()
            {
                player = GameHandler.playerEntity,
                playerBounds = playerBounds.volume,
                playerHealth = playerHealth,
                commandBuffer = commandBuffer
            };
            JobHandle jobHandle = pToHJob.Schedule(this, inputDeps);

            EntityQuery bulletQuery = EntityManager.CreateEntityQuery(typeof(Bullet), ComponentType.ReadOnly<BoundingVolume>(), ComponentType.ReadOnly<HealthModifier>());
            bulletQuery.AddDependency(jobHandle);

            BulletToDamageableCollisionJob bToDam = new BulletToDamageableCollisionJob()
            {
                commandBuffer = commandBuffer,
                bulletColliders = bulletQuery.ToComponentDataArray<BoundingVolume>(Allocator.TempJob),
                bulletHealthMods = bulletQuery.ToComponentDataArray<HealthModifier>(Allocator.TempJob),
                bulletInfos = bulletQuery.ToComponentDataArray<Bullet>(Allocator.TempJob),
                bullets = bulletQuery.ToEntityArray(Allocator.TempJob)
            };
            jobHandle = bToDam.Schedule(this, jobHandle);

            ValidateLifeJob validateLifeJob = new ValidateLifeJob()
            {
                playerEntity = GameHandler.playerEntity,
                player = player,
                commandBuffer = commandBuffer
            };
            jobHandle = validateLifeJob.Schedule(this, jobHandle);

            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
            return jobHandle;
        }
    }
}
