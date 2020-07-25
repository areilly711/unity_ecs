using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
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
                translation.Value += direction.value * speed.value * deltaTime;
            }).Schedule();
        }
    }
}