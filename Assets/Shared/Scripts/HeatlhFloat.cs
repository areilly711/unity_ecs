using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct HealthFloat : IComponentData
    {
        public float curr;
        public float max;
    }
}
