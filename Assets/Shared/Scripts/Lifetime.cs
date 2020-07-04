using Unity.Entities;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct Lifetime : IComponentData
    {
        public float value;
    }
}
