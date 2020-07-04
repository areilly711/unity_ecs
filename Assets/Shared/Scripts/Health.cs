using Unity.Entities;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct Health : IComponentData
    {
        public int curr;
        public int max;
    }
}

