using System;
using Unity.Entities;

namespace Shared
{ 
    [GenerateAuthoringComponent]
    public struct Radius : IComponentData
    {
        public float value;
    }
}
