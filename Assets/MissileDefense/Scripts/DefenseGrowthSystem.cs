using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Shared;

namespace MissileDefense
{
    public class DefenseGrowthSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = World.Time.DeltaTime;

            // use a sine wave to change the scale of the defense over time
            Entities.ForEach((ref NonUniformScale scale, ref Radius radius, ref Wave wave) =>
            {
                scale.Value = wave.amplitude * math.sin(wave.frequency * wave.time + wave.phase);
                radius.value = scale.Value.y;
                wave.time += dt; 

            }).Schedule();
        }
    }
}