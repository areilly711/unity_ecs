using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Shared
{
    public class LifetimeCountdownSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = World.Time.DeltaTime;
            Entities.ForEach((ref Lifetime lifetime, ref DeletionMark mark) =>
            {
                lifetime.value -= dt;
                if (lifetime.value <= 0)
                {
                    mark.value = 1;
                }
            }).Schedule();
        }
    }
}