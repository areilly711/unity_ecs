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
        public float spawnRate;
        public int spawns;
        public float speedMin;
        public float speedMax;
        public float posMin;
        public float posMax;
        //public float reloadRate;
    }
}
