using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct DeletionMark : IComponentData
    {
        public int value;
    }
}