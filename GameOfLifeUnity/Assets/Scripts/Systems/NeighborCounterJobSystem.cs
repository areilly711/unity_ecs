using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

//namespace GameLife
//{
//    public class NeighborCounterSystem : JobComponentSystem
//    {
//        /*
//        [BurstCompile]
//        struct NeighborCounterJob : IJobParallelFor
//        {
//            [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;
//            [ReadOnly] public ArchetypeChunkComponentType<Neighbors> neighborsType;
//            public ArchetypeChunkComponentType<LifeStatus> statusType;

//            public void Execute(int index)
//            {
//                var chunk = Chunks[index];
//                var chunkNeighbors = chunk.GetNativeArray(neighborsType);
//                var chunkStatus = chunk.GetNativeArray(statusType);
//                var instanceCount = chunk.Count;

//                NativeArray<Entity> neighbors = new NativeArray<Entity>(8, Allocator.Temp);
//                byte numLiveNeighbors = 0;

//                for (int i = 0; i < instanceCount; i++)
//                {
//                    // reset
//                    numLiveNeighbors = 0;

//                    Neighbors cell = chunkNeighbors[i];
//                    LifeStatus life = chunkStatus[i];

//                    neighbors[0] = cell.nw;
//                    neighbors[1] = cell.n;
//                    neighbors[2] = cell.ne;
//                    neighbors[3] = cell.w;
//                    neighbors[4] = cell.e;
//                    neighbors[5] = cell.sw;
//                    neighbors[6] = cell.s;
//                    neighbors[7] = cell.se;

//                    for (int j = 0; j < neighbors.Length; j++)
//                    {
//                        if (neighbors[j] != Entity.Null)
//                        {
//                            LifeStatus neighborStatus = chunkStatus[neighbors[j].Index];
//                            numLiveNeighbors += neighborStatus.isAliveNow;
//                        }
//                    }

//                    if (life.isAliveNow == 1)
//                    {
//                        if (numLiveNeighbors < 2 || numLiveNeighbors > 3)
//                        {
//                            life.isAliveNextCycle = 0;
//                        }
//                        else
//                        {
//                            life.isAliveNextCycle = 1;
//                        }
//                    }
//                    else
//                    {
//                        if (numLiveNeighbors == 3)
//                        {
//                            life.isAliveNextCycle = 1;
//                        }
//                        else
//                        {
//                            life.isAliveNextCycle = 0;
//                        }

//                    }
//                }
//            }
//        }
//        */

       
        
        
        

//        float timePassed = 0.5f;
//        const float UpdateInterval = 0.5f;
//        public bool forceJob;
//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            if (timePassed <= UpdateInterval && !forceJob)
//            {
//                timePassed += Time.deltaTime;
//                return inputDeps;
//            }

//            timePassed = 0;
//            forceJob = false;
//            return PerformJob(inputDeps);
//        }

//        public JobHandle PerformJob(JobHandle inputDeps)
//        {
//            var query = new EntityQueryDesc
//            {
//                All = new ComponentType[] { typeof(LifeStatus), ComponentType.ReadOnly<Neighbors>() }
//            };

//            EntityQuery group = GetEntityQuery(query);

//            NativeArray<LifeStatus> lifeStatusArray = group.ToComponentDataArray<LifeStatus>(Allocator.TempJob);
//            //var neighbors = GetArchetypeChunkComponentType<Neighbors>(true);
//            //var status = GetArchetypeChunkComponentType<LifeStatus>();
//            //var chunks = group.CreateArchetypeChunkArray(Allocator.TempJob);

//            //ArchetypeChunkArray chunkArray = new ArchetypeChunkArray();
//            //ArchetypeChunkArray.CalculateEntityCount(chunks);
//            //NativeArray<>

//            NeighborCounterJob job = new NeighborCounterJob()
//            {
//                //Chunks = chunks,
//                //neighborsType = neighbors,
//                //statusType = status
//                lifeStatusArray = lifeStatusArray
//            };

//            //for (int i = 0; i < chunks.Length; i++)
//            //{
//            //    Debug.Log(chunks[i].Length);
//            //}

//            //JobHandle jobHandle = job.Schedule(chunks.Length, 32, inputDeps);
//            JobHandle jobHandle = job.Schedule(this, inputDeps);
//            return jobHandle;
//        }
//    }
//}
