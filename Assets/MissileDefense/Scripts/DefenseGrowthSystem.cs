using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Shared;
using UnityEditor;

namespace MissileDefense
{
    public class DefenseGrowthSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float dt = World.Time.DeltaTime;

            Entities
                .ForEach((ref NonUniformScale scale, ref Radius radius, ref Wave wave) =>
            {
                scale.Value = wave.amplitude * math.sin(wave.frequency * wave.time + wave.phase);
                radius.value = scale.Value.y;
                wave.time += dt; 

            }).Schedule();
        }
    }
}