using Unity.Entities;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct AttackSpeed : IComponentData
    {
        public float speed;
        public float counter;
    }
}
