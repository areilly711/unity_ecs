using Unity.Entities;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct Score : IComponentData
    {
        public int value;
    }
}