using Unity.Entities;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct HealthInt : IComponentData
    {
        public int curr;
        public int max;
    }
}

