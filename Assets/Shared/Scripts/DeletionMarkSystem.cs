using Unity.Entities;
using Unity.Jobs;

namespace Shared
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(LifetimeCountdownSystem))]
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