using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Authoring;
using Shared;
using Breakout;

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
            
            //Entities.WithAll<Player>().ForEach((ref PhysicsVelocity vel, in Speed speed) => {
            //    vel.Linear.y += dt * movement * speed.value;               
            //    //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
            //}).Schedule();

            Entities.WithAll<Player, PongPaddle>().ForEach((ref Translation translation, in Speed speed) => {                
                translation.Value.y +=  dt * movement * speed.value;
                translation.Value.y = math.clamp(translation.Value.y, -4, 4);
            }).Schedule();
        }
    }
}
