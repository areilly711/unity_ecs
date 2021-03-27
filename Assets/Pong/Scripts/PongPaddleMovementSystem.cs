using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Shared;

namespace Pong
{
    public class PongPaddleMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float movement = Input.GetAxis("Horizontal") * -1;
            if (movement == 0)
            {
                movement = Input.GetAxis("Vertical");
            }
            float dt = Time.DeltaTime;
            
            Entities.WithAll<Player, PongPaddle>().ForEach((ref Translation translation, in Speed speed) => {                
                translation.Value.y +=  dt * movement * speed.value;
                translation.Value.y = math.clamp(translation.Value.y, -4, 4);
            }).Schedule();
        }
    }
}
