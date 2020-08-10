using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Shared;

namespace Breakout
{
    public class BreakoutPaddleMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float movement = Input.GetAxis("Horizontal") * 1;
            float dt = Time.DeltaTime;
                        
            Entities.WithAll<BreakoutPaddle>().ForEach((ref Translation translation, in Speed speed) => {                
                translation.Value.x +=  dt * movement * speed.value;
                translation.Value.x = math.clamp(translation.Value.x, -5.5f, 5.5f);
            }).Schedule();
        }
    }
}
