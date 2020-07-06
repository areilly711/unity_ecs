using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Shared
{
    public class AttackSpeedReloadSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = World.Time.DeltaTime;
            Entities.ForEach((ref AttackSpeed attackSpeed, ref Ready ready) =>
            {
                // only increment if it's not ready
                attackSpeed.counter = math.select(attackSpeed.counter + dt, 0, ready.value);

                // change readiness if the counter has surpased the required amount
                ready.value = ready.value || attackSpeed.counter >= attackSpeed.speed;
            }).Schedule();
        }
    }
}