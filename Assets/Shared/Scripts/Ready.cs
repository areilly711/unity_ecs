using System;
using Unity.Entities;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct Ready : IComponentData
    {
        public bool value;
    }
}