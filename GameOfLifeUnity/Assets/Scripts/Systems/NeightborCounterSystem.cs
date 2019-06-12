using UnityEngine;
using Unity.Entities;
using Unity.Collections;
/*
namespace GameLife
{
    public class NeighborCounterSystem : ComponentSystem
    {
        float timePassed = 0.5f;
        const float UpdateInterval = 0.5f;

        protected override void OnUpdate()
        {
            if (timePassed <= UpdateInterval)
            {
                timePassed += UnityEngine.Time.deltaTime;
                return;// inputDeps;
            }

            timePassed = 0;

            EntityQuery allCell = EntityManager.CreateEntityQuery(typeof(Neighbors));
            NativeArray<Entity> neighbors = new NativeArray<Entity>(8, Allocator.Temp);
            byte numLiveNeighbors = 0;

            Entities.WithAllReadOnly<Neighbors, LifeStatus>().ForEach((Entity e, ref Neighbors cell, ref LifeStatus life) =>
            {
                // reset
                numLiveNeighbors = 0;

                neighbors[0] = cell.nw;
                neighbors[1] = cell.n;
                neighbors[2] = cell.ne;
                neighbors[3] = cell.w;
                neighbors[4] = cell.e;
                neighbors[5] = cell.sw;
                neighbors[6] = cell.s;
                neighbors[7] = cell.se;

                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (neighbors[i] != Entity.Null)
                    {
                        LifeStatus neighborStatus = EntityManager.GetComponentData<LifeStatus>(neighbors[i]);
                        numLiveNeighbors += neighborStatus.isAliveNow;
                    }
                }
                
                if (life.isAliveNow == 1)
                {
                    if (numLiveNeighbors < 2 || numLiveNeighbors > 3)
                    {
                        life.isAliveNextCycle = 0;
                    }
                    else
                    {
                        life.isAliveNextCycle = 1;
                    }
                }
                else
                {
                    if (numLiveNeighbors == 3)
                    {
                        life.isAliveNextCycle = 1;
                    }
                    else
                    {
                        life.isAliveNextCycle = 0;
                    }
                    
                }
            });

            neighbors.Dispose();
        }
    }
}
*/
