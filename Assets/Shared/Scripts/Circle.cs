using Unity.Entities;
using Unity.Mathematics;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct Circle : IComponentData
    {
        public float2 position;
        public float radius;
    }
}
