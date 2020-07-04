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

    public struct Enemy : IComponentData
    {
        public int points;
    }

    public struct Boid : IComponentData
    {

    }

    public struct PlayerSystemState : ISystemStateComponentData
    {

    }

    public struct Food : IComponentData { }
    public struct Player : IComponentData
    {
        public int score;
    }
        
}
