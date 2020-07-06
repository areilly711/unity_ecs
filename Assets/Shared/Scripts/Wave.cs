using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Shared
{
    [GenerateAuthoringComponent]
    public struct Wave : IComponentData
    {
        // the peak deviation of the function from zero.
        public float amplitude;
        
        // the number of oscillations (cycles) that occur each second of time.
        public float frequency;

        // specifies (in radians) where in its cycle the oscillation is at t = 0
        public float phase;

        // current time of the wave
        public float time;
    }
}