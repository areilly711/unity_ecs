using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

namespace GameLife
{
    public class PointInCellAABSystem : JobComponentSystem
    {
        [BurstCompile]
        public struct TestClickInAABBJob : IJobForEachWithEntity<BoundingBox, Scale, LifeStatus, ClickStatus>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float> scaleConsts;
            //public float3 point;
            public Bounds point;
            public void Execute(Entity entity, int index, ref BoundingBox b, ref Scale s, ref LifeStatus life, ref ClickStatus click)
            {
                if (!click.clicked && b.box.Intersects(point))
                {
                    click.clicked = true;
                    life.isAliveNow ^= 1;
                    s.Value = scaleConsts[life.isAliveNow];
                }
            }
        }

        [BurstCompile]
        public struct ResetClickJob : IJobForEach<ClickStatus>
        {
            public void Execute(ref ClickStatus c)
            {
                c.clicked = false;
            }
        }

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
                Bounds clickArea = new Bounds();
                clickArea.center = new float3(converted.x, converted.y, 0);
                clickArea.extents = new float3(0.05f, 0.05f, 1);
                //Debug.Log(clickArea.Center + " " + clickArea.Extents);
                TestClickInAABBJob job = new TestClickInAABBJob()
                {
                    scaleConsts = scaleConsts,
                    //point = r.origin
                    point = clickArea
                };

                JobHandle jobHandle = job.Schedule(this, inputDeps);
                return jobHandle;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ResetClickJob job = new ResetClickJob();
                JobHandle jobHandle = job.Schedule(this, inputDeps);
                return jobHandle;
            }
            else
            {
                return inputDeps;
            }
        }
    }
}

public struct BoundingBox : IComponentData
{
    public Bounds box;
}

public struct ClickStatus : IComponentData
{
    public bool clicked;
}