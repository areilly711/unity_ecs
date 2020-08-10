using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Physics.Extensions;
using Shared;

namespace Breakout
{
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class BrickCollisionSystem : SystemBase
    {
        BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        StepPhysicsWorld m_StepPhysicsWorldSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        [BurstCompile]
        struct CollisionJob : ICollisionEventsJob
        {
            public ComponentDataFromEntity<HealthInt> brickHealths; 
            public void Execute(CollisionEvent e)
            {
                Entity healthEntity = Entity.Null;
                if (brickHealths.HasComponent(e.EntityA))
                {
                    healthEntity = e.EntityA;                    
                }
                else if (brickHealths.HasComponent(e.EntityB))
                {
                    healthEntity = e.EntityB;
                }

                if (healthEntity != Entity.Null)
                {
                    HealthInt health = brickHealths[healthEntity];
                    health.curr -= 1;
                    brickHealths[healthEntity] = health;
                }
            }
        }

        protected override void OnUpdate()
        {
            if (EntityManager.CreateEntityQuery(typeof(Brick)).CalculateEntityCount() == 0)
            {
                return;
            }

            Dependency = new CollisionJob()
            {
                brickHealths = GetComponentDataFromEntity<HealthInt>()                
            }.Schedule(m_StepPhysicsWorldSystem.Simulation,
            ref m_BuildPhysicsWorldSystem.PhysicsWorld, Dependency);
            Dependency.Complete();
        }
    }
}