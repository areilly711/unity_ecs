using Unity.Entities;

namespace GameLife
{

    public struct LifeStatus : IComponentData
    {
        public byte isAlive;
    }

    public struct LifeStatusNextCycle : IComponentData
    {
        public byte isAlive;
    }
}
