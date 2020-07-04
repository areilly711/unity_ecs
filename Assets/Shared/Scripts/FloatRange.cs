using Unity.Entities;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct FloatRange : IComponentData
    {
        public float min;
        public float max;
    }
}