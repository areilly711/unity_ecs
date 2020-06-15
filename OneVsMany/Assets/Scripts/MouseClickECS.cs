using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

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
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<BoundingBox>(e, new BoundingBox { box = aabb });
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<Scale>(e, new Scale { Value = startScale });
                World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData<Translation>(e, new Translation { Value = pos });
            }
        }
       
    }
}
