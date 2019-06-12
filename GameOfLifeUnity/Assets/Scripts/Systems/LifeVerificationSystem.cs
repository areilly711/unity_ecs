using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

namespace GameLife
{
    /// <summary>
    /// Responsible for the game logic of Conway's Game of Life
    /// https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life
    /// </summary>
    public class LifeVerificationSystem : JobComponentSystem
    {
        /// <summary>
        /// Counts the number of live neighbors a cell has and then determines if it should live or die next cycle
        /// </summary>
        [BurstCompile]
        struct NeighborCounterJob : IJobForEach<Neighbors, LifeStatus>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<LifeStatus> lifeStatusArray;
            public int firstCellOffset;

            public void Execute([ReadOnly] ref Neighbors cell, ref LifeStatus life)
            {
                NativeArray<Entity> neighbors = new NativeArray<Entity>(8, Allocator.Temp);
                byte numLiveNeighbors = 0;

                // stick all the neighbors in the array for easy iteration
                neighbors[0] = cell.nw;
                neighbors[1] = cell.n;
                neighbors[2] = cell.ne;
                neighbors[3] = cell.w;
                neighbors[4] = cell.e;
                neighbors[5] = cell.sw;
                neighbors[6] = cell.s;
                neighbors[7] = cell.se;

                for (int j = 0; j < neighbors.Length; j++)
                {
                    if (neighbors[j] != Entity.Null)
                    {
                        LifeStatus neighborStatus = lifeStatusArray[neighbors[j].Index];
                        numLiveNeighbors += neighborStatus.isAliveNow;
                    }
                }

                if (life.isAliveNow == 1) // the cell currently alive
                {
                    if (numLiveNeighbors < 2 || numLiveNeighbors > 3)
                    {
                        // die from under population or over population
                        life.isAliveNextCycle = 0;
                    }
                    else
                    {
                        life.isAliveNextCycle = 1;
                    }
                }
                else // the cell is currently dead
                {
                    if (numLiveNeighbors == 3)
                    {
                        // become alive from reproduction
                        life.isAliveNextCycle = 1;
                    }
                    else
                    {
                        life.isAliveNextCycle = 0;
                    }

                }

                neighbors.Dispose();
            }
        }

        /// <summary>
        /// Sets the cell as dead or alive based on the previous calculation in the Neighborcounter job
        /// </summary>
        [BurstCompile]
        struct LifeStatusChangerJob : IJobForEachWithEntity<LifeStatus, Scale>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float> scaleConsts;

            public void Execute(Entity entity, int index, ref LifeStatus status, [ReadOnly] ref Scale scale)
            {
                status.isAliveNow = status.isAliveNextCycle;

                // dead cells are invisible (scale 0)
                scale.Value = scaleConsts[status.isAliveNow];
            }
        }

        float timePassed = 0;
        const float UpdateInterval = 0.5f;
        public bool forceJob;
        //public int firstCellOffset;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (timePassed <= UpdateInterval && !forceJob)
            {
                // not enough time has passed, get out
                timePassed += UnityEngine.Time.deltaTime;
                return inputDeps;
            }

            // the job was forced or it's time to update to the next cell generation
            timePassed = 0;
            forceJob = false;
            return PerformJob(inputDeps);
        }

        public JobHandle PerformJob(JobHandle inputDeps)
        {
            var query = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(LifeStatus), ComponentType.ReadOnly<Neighbors>() }
            };

            //ArchetypeChunkComponentType n = GetArchetypeChunkComponentType<Neighbors>(true);
            
            EntityQuery group = GetEntityQuery(query);

            /*
            var chunks = group.CreateArchetypeChunkArray(Allocator.TempJob);
            int numCells = ArchetypeChunkArray.CalculateEntityCount(chunks);
            NativeArray<LifeStatus> ls = new NativeArray<LifeStatus>(numCells, Allocator.TempJob);
            var status = GetArchetypeChunkComponentType<LifeStatus>();
            int totalArrayIndex = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                var tempArray = chunks[i].GetNativeArray<LifeStatus>(status);
                for (int j = 0; j < tempArray.Length; j++)
                {
                    ls[totalArrayIndex] = tempArray[j];
                    totalArrayIndex += 1;
                }
            }
            chunks.Dispose();
            */

            //EntityManager.get
            
            NativeArray<LifeStatus> lifeStatusArray = group.ToComponentDataArray<LifeStatus>(Allocator.TempJob);
            NeighborCounterJob neighborCounterJob = new NeighborCounterJob()
            {
                lifeStatusArray = lifeStatusArray,// lifeStatusArray,
            };
            JobHandle jobHandle = neighborCounterJob.Schedule(this, inputDeps);
            //JobHandle jobHandle = neighborCounterJob.Schedule(chunks.Length, 32, inputDeps);

            // get the scaling constants and save them for our job later. This way, we can do a look up rather than a conditional
            EntityQuery scaleConstQuery = EntityManager.CreateEntityQuery(typeof(ScaleConst), typeof(Scale));
            NativeArray<Scale> consts = scaleConstQuery.ToComponentDataArray<Scale>(Allocator.TempJob);
            NativeArray<float> scaleConsts = new NativeArray<float>(consts.Length, Allocator.TempJob);
            for (int i = 0; i < consts.Length; i++)
            {
                scaleConsts[i] = consts[i].Value;
            }
            consts.Dispose();

            // update the life status of all the cells with this job
            LifeStatusChangerJob lifeChangerJob = new LifeStatusChangerJob
            {
                scaleConsts = scaleConsts
            };

            // this job requires the neighbor counter job to finish first
            jobHandle = lifeChangerJob.Schedule(this, jobHandle);

            return jobHandle;
        }
    }
}
