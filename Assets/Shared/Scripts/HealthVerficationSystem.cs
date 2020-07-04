using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Shared
{


    public class HealthVerficationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref DeletionMark mark, in Health health) =>
            {
                mark.value = math.select(0, 1, health.curr <= 0);
            }).Schedule();
        }
    }
}