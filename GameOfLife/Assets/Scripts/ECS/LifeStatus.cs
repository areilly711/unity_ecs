using Unity.Entities;

namespace GameLife
{
    //public struct LifeStatus : IComponentData
    //{
    //    public byte isAliveNow;
    //    public byte isAliveNextCycle;
    //}

    public struct LifeStatus : IComponentData
    {
        public byte isAliveNow;
    }

    public struct LifeStatusNextCycle : IComponentData
    {
        public byte isAliveNextCycle;
    }
}
