using Unity.Entities;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct Damage : IComponentData
    {
        public float value;
    }
}
