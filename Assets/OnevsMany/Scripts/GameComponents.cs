/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using Unity.Entities;
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
