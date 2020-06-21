using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PointInAABSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 converted = r.origin;

            float3 point = new float3(converted.x, converted.y, 0.5f);

            //JobHandle jobHandle = job.Schedule(this, inputDeps);
            JobHandle jobHandle = Entities.ForEach((Entity entity, int entityInQueryIndex, ref BoundingBox b, ref Scale s) =>
            {
                if (b.aabb.Contains(point))
                {
                    s.Value += 0.1f;
                    b.aabb.Extents = s.Value * 0.5f;
                }
            }).Schedule(inputDeps);
            return jobHandle;
        }
        else
        {
            return inputDeps;
        }
    }
}