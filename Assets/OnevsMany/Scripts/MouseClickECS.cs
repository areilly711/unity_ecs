/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Shared;

public class MouseClickECS : MonoBehaviour
{
    public int numRows = 10;
    public int numCols = 10;
    public float startScale = 1;
    public Mesh m;
    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                Entity e = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(
                   typeof(Translation),
                   typeof(LocalToWorld),
                   typeof(Scale),
                   typeof(RenderMesh),
                   typeof(BoundingBox)
               );

                AABB aabb = new AABB();
                //float3 pos = new float3(i - 5, j - 5, 0);
                float3 pos = new float3(0);
                aabb.Center = new float3(pos);
                aabb.Extents = new float3(startScale * 0.5f);
                World.DefaultGameObjectInjectionWorld.EntityManager.SetSharedComponentData<RenderMesh>(e, new RenderMesh { mesh = m, material = mat });
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<BoundingBox>(e, new BoundingBox { aabb = aabb });
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<Scale>(e, new Scale { Value = startScale });
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<Translation>(e, new Translation { Value = pos });
            }
        }
       
    }
}
