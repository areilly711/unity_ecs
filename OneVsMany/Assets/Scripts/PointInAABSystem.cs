using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;


public class PointInAABSystem : JobComponentSystem
{
    [BurstCompile]
    public struct PointInAABBJob : IJobForEachWithEntity<BoundingBox, Scale>
    {
        public float3 point;
        public void Execute(Entity entity, int index, ref BoundingBox b, ref Scale s)
        {
            if (b.box.Contains(point))
            {
                s.Value += 0.1f;
                b.box.Extents = s.Value * 0.5f;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 converted = r.origin;
            PointInAABBJob job = new PointInAABBJob()
            {
                point = new float3(converted.x, converted.y, 0.5f)
            };

            JobHandle jobHandle = job.Schedule(this, inputDeps);
            return jobHandle;
        }
        else
        {
            return inputDeps;
        }
    }
}

public struct BoundingBox : IComponentData
{
    public AABB box;
}