using Shared;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PongAIPaddleMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        // get the ball's position
        float3 ballPos = EntityManager.CreateEntityQuery(typeof(Translation), typeof(Ball)).GetSingleton<Translation>().Value;
        
        Entities
            .WithAll<AI>()
            .ForEach((ref Translation translation, in Speed speed) => {
                float dist = math.abs(ballPos.y - translation.Value.y);
                float dir = math.select(-1, 1, ballPos.y > translation.Value.y);
                dir = math.select(0, dir, dist > 0.3f);
                translation.Value.y +=  speed.value * deltaTime * dir;
                translation.Value.y = math.clamp(translation.Value.y, -4, 4);
            }).Schedule();
    }
}
