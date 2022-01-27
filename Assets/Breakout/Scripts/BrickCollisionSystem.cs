/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
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