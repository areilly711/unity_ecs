using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Shared
{
    public class DeletionMarkSystem : SystemBase
    {
        protected override void OnUpdate()
        {

            Entities
                .WithStructuralChanges()
                .WithoutBurst()
                .ForEach((Entity e, in DeletionMark mark) =>
            {
                if (mark.value != 0)
                {
                    EntityManager.DestroyEntity(e);
                }
            }).Run();
            //handle.Complete();
        }
    }
}