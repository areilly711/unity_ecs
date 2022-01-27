/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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