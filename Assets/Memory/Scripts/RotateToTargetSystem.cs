using Memory;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Memory
{
    public class RotateToTargetSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = EntityManager.World.Time.DeltaTime;

            Entities.ForEach((ref Rotation rotation, ref Timer timer, in TargetRotation targetRot) =>
            {
                rotation.Value = math.slerp(rotation.Value, targetRot.target, timer.curr / timer.max);
                timer.curr += dt;
                timer.curr = math.min(timer.curr, timer.max);
            }).Schedule();
        }
    }
}