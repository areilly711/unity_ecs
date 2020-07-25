using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Shared;

namespace GameLife
{
    /// <summary>
    /// Tests where you are clicking with the mouse in order to set cells alive or dead
    /// </summary>
    public class PointInCellAABSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Input.GetMouseButton(0))
            {
                // Query for scales
                EntityQuery scaleConstQuery = EntityManager.CreateEntityQuery(typeof(ScaleConst), typeof(Scale));
                NativeArray<Scale> consts = scaleConstQuery.ToComponentDataArray<Scale>(Allocator.TempJob);
                NativeArray<float> scaleConsts = new NativeArray<float>(consts.Length, Allocator.TempJob);
                for (int i = 0; i < consts.Length; i++)
                {
                    scaleConsts[i] = consts[i].Value;
                }
                consts.Dispose();
                
                // create the ray to test against AABBs
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 converted = r.origin;
                AABB clickArea = new AABB();
                clickArea.Center = new float3(converted.x, converted.y, 0);
                clickArea.Extents = new float3(0.05f, 0.05f, 1);
                
                JobHandle jobHandle = Entities
                    .WithDeallocateOnJobCompletion(scaleConsts)
                    .ForEach((Entity e, int entityInQueryIndex, ref BoundingBox b, ref Scale s, ref LifeStatus life, ref ClickStatus click) =>
                {
                    if (!click.clicked && b.aabb.Contains(clickArea))
                    {
                        click.clicked = true;
                        life.isAlive ^= 1;
                        s.Value = scaleConsts[life.isAlive];
                    }
                }).Schedule(inputDeps);

                return jobHandle;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                JobHandle jobHandle = Entities
                    .ForEach((ref ClickStatus click) =>
                    {
                        click.clicked = false;
                    }).Schedule(inputDeps);

                return jobHandle;
            }
            else
            {
                return inputDeps;
            }
        }
    }
}