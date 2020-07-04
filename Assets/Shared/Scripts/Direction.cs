using Unity.Entities;
using Unity.Mathematics;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct Direction : IComponentData
    {
        public float3 value;
    }   
}
