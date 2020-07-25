using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace GameLife
{
    /// <summary>
    /// Responsible for the game logic of Conway's Game of Life
    /// https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life
    /// </summary>
    public class LifeVerificationSystem : JobComponentSystem
    {        
        /// <summary>
        /// keeps track of the seconds that have passed
        /// </summary>
        float timePassed = 0;

        /// <summary>
        /// The interval (in seconds) before moving to the next generation
        /// </summary>
        const float UpdateInterval = 0.5f;
        public bool forceJob;
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (timePassed <= UpdateInterval && !forceJob)
            {
                // not enough time has passed, get out
                timePassed += World.Time.DeltaTime;
                return inputDeps;
            }

            // the job was forced or it's time to update to the next cell generation
            timePassed = 0;
            forceJob = false;
            return PerformJob(inputDeps);
        }

        public JobHandle PerformJob(JobHandle inputDeps)
        {
            ComponentDataFromEntity<LifeStatus> lifeStatusLookup = GetComponentDataFromEntity<LifeStatus>(true);
            
            JobHandle jobHandle = Entities
                .WithReadOnly(lifeStatusLookup)
                .ForEach((Entity cell, ref LifeStatusNextCycle next, in Neighbors neighbors) =>
            {
                byte numLiveNeighbors = 0;

                // check current life status of all neighbors                
                if (neighbors.nw != Entity.Null) numLiveNeighbors += lifeStatusLookup[neighbors.nw].isAlive;
                if (neighbors.n != Entity.Null) numLiveNeighbors += lifeStatusLookup[neighbors.n].isAlive;
                if (neighbors.ne != Entity.Null) numLiveNeighbors += lifeStatusLookup[neighbors.ne].isAlive;
                if (neighbors.w != Entity.Null) numLiveNeighbors += lifeStatusLookup[neighbors.w].isAlive;
                if (neighbors.e != Entity.Null) numLiveNeighbors += lifeStatusLookup[neighbors.e].isAlive;
                if (neighbors.sw != Entity.Null) numLiveNeighbors += lifeStatusLookup[neighbors.sw].isAlive;
                if (neighbors.s != Entity.Null) numLiveNeighbors += lifeStatusLookup[neighbors.s].isAlive;
                if (neighbors.se != Entity.Null) numLiveNeighbors += lifeStatusLookup[neighbors.se].isAlive;
                

                if (lifeStatusLookup[cell].isAlive == 1) // the cell currently alive
                {
                    next.isAlive = (byte)math.select(1, 0, numLiveNeighbors < 2 || numLiveNeighbors > 3);
                    /*if (numLiveNeighbors < 2 || numLiveNeighbors > 3)
                    {
                        // die from under population or over population
                        next.isAlive = 0;
                    }
                    else
                    {
                        next.isAlive = 1;
                    }*/
                }
                else // the cell is currently dead
                {
                    next.isAlive = (byte)math.select(0, 1, numLiveNeighbors == 3);
                    /*if (numLiveNeighbors == 3)
                    {
                        // become alive from reproduction
                        next.isAlive = 1;
                    }
                    else
                    {
                        next.isAlive = 0;
                    }*/
                }
            }).Schedule(inputDeps);
            
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
            // this job requires the neighbor counter job to finish first
            jobHandle = Entities
                .WithReadOnly(scaleConsts)
                .WithDeallocateOnJobCompletion(scaleConsts)
                .ForEach((Entity entity, int entityInQueryIndex, ref Scale scale, ref LifeStatus status, in LifeStatusNextCycle nextStatus) =>
            {
                status.isAlive = nextStatus.isAlive;

                // dead cells are invisible (scale 0)
                scale.Value = scaleConsts[status.isAlive];

            }).Schedule(jobHandle);
            
            return jobHandle;
        }
    }
}
