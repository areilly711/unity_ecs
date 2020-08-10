using System;
using Unity.Entities;

namespace Memory
{
    [Serializable]
    public struct Card : IComponentData
    {
        public int value;
        public Entity face;
    }
}