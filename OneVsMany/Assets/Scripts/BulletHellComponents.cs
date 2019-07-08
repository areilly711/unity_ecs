using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace OneVsMany
{
    public struct Movement : IComponentData
    {
        public float3 direction;
        public float speed;
    }

    public struct Health : IComponentData
    {
        public float curr;
        public float max;
    }

    public struct BoundingVolume : IComponentData
    {
        public Bounds volume;
    }

    public struct HealthModifier : IComponentData
    {
        public float value;
    }

    // Tags
    public struct Bullet : IComponentData
    {
        public bool isActive;
        public float age;
    }

    public struct Boid : IComponentData
    {

    }

    public struct Food : IComponentData { }
    public struct Player : IComponentData { }
    public struct Enemy : IComponentData { }    
}
