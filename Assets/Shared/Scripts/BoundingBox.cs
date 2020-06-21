using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct BoundingBox : IComponentData
{
    public Bounds box;
    public AABB aabb;
}