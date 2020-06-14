using UnityEngine;
using Unity.Entities;

[System.Serializable]
public struct BoundingBox : IComponentData
{
    public Bounds box;
}