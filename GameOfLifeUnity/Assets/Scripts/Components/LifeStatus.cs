using Unity.Entities;

namespace GameLife
{
    public struct LifeStatus : IComponentData
    {
        public byte isAliveNow;
        public byte isAliveNextCycle;
    }
}
