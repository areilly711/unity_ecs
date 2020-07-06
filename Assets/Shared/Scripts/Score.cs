using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct Score : IComponentData
    {
        public int value;
    }
}