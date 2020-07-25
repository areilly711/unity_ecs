using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Memory
{
    [Serializable]
    public struct Card : IComponentData
    {
        public int value;
        public Entity face;
    }
}