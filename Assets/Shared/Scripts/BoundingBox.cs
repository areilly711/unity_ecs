﻿using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Shared
{
    [System.Serializable]
    [GenerateAuthoringComponent]
    public struct BoundingBox : IComponentData
    {
        public AABB aabb;
    }
}
