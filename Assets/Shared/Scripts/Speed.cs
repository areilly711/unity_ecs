using Unity.Entities;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct Speed : IComponentData
    {
        public float value;
    }
}
