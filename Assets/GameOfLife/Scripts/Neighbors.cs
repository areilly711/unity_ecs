using Unity.Entities;

namespace GameLife
{
    /// <summary>
    /// Every cell can have up to 8 neighbors in different directions (n = north, s = south, w = west, e = east)
    /// </summary>
    public struct Neighbors : IComponentData
    {
        public Entity nw;
        public Entity n;
        public Entity ne;
        public Entity w;
        public Entity e;
        public Entity sw;
        public Entity s;
        public Entity se;
    }
}