using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Shared;


namespace MissileDefense
{
    public class MissileMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = World.Time.DeltaTime;
            Entities.ForEach((ref Translation translation, in Direction direction, in Speed speed) =>
            {
                // move straight along the missile direction
                translation.Value += direction.value * speed.value * deltaTime;
            }).Schedule();
        }
    }
}