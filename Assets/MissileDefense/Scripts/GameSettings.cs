using Unity.Entities;
using Unity.Mathematics;

namespace MissileDefense
{
    /// <summary>
    /// Difficulty settings for the game
    /// </summary>
    [GenerateAuthoringComponent]
    public class GameSettings : IComponentData
    {
        /// <summary>
        /// how quickly the missiles spawn, in seconds
        /// </summary>
        public float spawnRate;

        /// <summary>
        /// the number of spawns when the spawn rate is ready
        /// </summary>
        public int spawns;

        /// <summary>
        /// minimum missile speed
        /// </summary>
        public float speedMin;

        /// <summary>
        /// maximum missile speed
        /// </summary>
        public float speedMax;

        /// <summary>
        /// minimum x pos to spawn
        /// </summary>
        public float posMin;

        /// <summary>
        /// maximum x pos to spawn
        /// </summary>
        public float posMax;

        /// <summary>
        /// starting missile y pos
        /// </summary>
        public float spawnYPos;

        /// <summary>
        /// destory the missile when its y pos drops below this value
        /// </summary>
        public float worldEndThreshold;        
    }
}
