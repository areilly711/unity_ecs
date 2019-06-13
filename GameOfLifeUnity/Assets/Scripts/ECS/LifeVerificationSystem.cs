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
        struct NeighborCounterJob : IJobForEachWithEntity<Neighbors, LifeStatusNextCycle>
        {
            [ReadOnly] public ComponentDataFromEntity<LifeStatus> lifeStatusLookup;

            public void Execute(Entity e, int index, [ReadOnly] ref Neighbors cell, [WriteOnly] ref LifeStatusNextCycle next)
            {
                byte numLiveNeighbors = 0;

                // check current life status of all neighbors
                if (cell.nw != Entity.Null) numLiveNeighbors += lifeStatusLookup[cell.nw].isAliveNow;
                if (cell.n != Entity.Null) numLiveNeighbors += lifeStatusLookup[cell.n].isAliveNow;
                if (cell.ne != Entity.Null) numLiveNeighbors += lifeStatusLookup[cell.ne].isAliveNow;
                if (cell.w != Entity.Null) numLiveNeighbors += lifeStatusLookup[cell.w].isAliveNow;
                if (cell.e != Entity.Null) numLiveNeighbors += lifeStatusLookup[cell.e].isAliveNow;
                if (cell.sw != Entity.Null) numLiveNeighbors += lifeStatusLookup[cell.sw].isAliveNow;
                if (cell.s != Entity.Null) numLiveNeighbors += lifeStatusLookup[cell.s].isAliveNow;
                if (cell.se != Entity.Null) numLiveNeighbors += lifeStatusLookup[cell.se].isAliveNow;
                
                if (lifeStatusLookup[e].isAliveNow == 1) // the cell currently alive
                {
                    if (numLiveNeighbors < 2 || numLiveNeighbors > 3)
                    {
                        // die from under population or over population
                        next.isAliveNextCycle = 0;
                    }
                    else
                    {
                        next.isAliveNextCycle = 1;
                    }
                }
                else // the cell is currently dead
                {
                    if (numLiveNeighbors == 3)
                    {
                        // become alive from reproduction
                        next.isAliveNextCycle = 1;
                    }
                    else
                    {
                        next.isAliveNextCycle = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the cell as dead or alive based on the previous calculation in the Neighborcounter job
        /// </summary>
        [BurstCompile]
        struct LifeStatusChangerJob : IJobForEachWithEntity<LifeStatus, LifeStatusNextCycle, Scale>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float> scaleConsts;

            public void Execute(Entity entity, int index, [WriteOnly] ref LifeStatus status, [ReadOnly] ref LifeStatusNextCycle nextStatus, [ReadOnly] ref Scale scale)
            {
                status.isAliveNow = nextStatus.isAliveNextCycle;

                // dead cells are invisible (scale 0)
                scale.Value = scaleConsts[status.isAliveNow];
            }
        }

        float timePassed = 0;
        const float UpdateInterval = 0.5f;
        public bool forceJob;
        
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
            
            EntityQuery group = GetEntityQuery(query);
            ComponentDataFromEntity<LifeStatus> statuses = GetComponentDataFromEntity<LifeStatus>(false);
            
            NeighborCounterJob neighborCounterJob = new NeighborCounterJob()
            {
                lifeStatusLookup = statuses,
            };
            JobHandle jobHandle = neighborCounterJob.Schedule(this, inputDeps);
            
            // get the scaling constants and save them for our job later. This way, we can do a look up rather than a conditional
            EntityQuery scaleConstQuery = EntityManager.CreateEntityQuery(typeof(ScaleConst), typeof(Scale));
            NativeArray<Scale> consts = scaleConstQuery.ToComponentDataArray<Scale>(Allocator.TempJob);
            NativeArray<float> scaleConsts = new NativeArray<float>(consts.Length, Allocator.TempJob);
            for (int i = 0; i < consts.Length; i++)
            {
                scaleConsts[i] = consts[i].Value;
            }
            consts.Dispose();

            // update the current life status of all the cells with this job
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
