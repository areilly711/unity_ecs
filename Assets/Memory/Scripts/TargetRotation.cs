using Unity.Entities;
using Unity.Mathematics;

namespace Memory
{
    [System.Serializable]
    [GenerateAuthoringComponent]
    public struct TargetRotation : IComponentData
    {
        /// <summary>
        /// target rotation
        /// </summary>
        public quaternion target;
    }
}