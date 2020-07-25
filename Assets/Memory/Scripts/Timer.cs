using Unity.Entities;

namespace Memory
{
    [System.Serializable]
    [GenerateAuthoringComponent]
    public struct Timer : IComponentData
    {
        public float curr;
        public float max;
    }
}
