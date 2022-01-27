/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

namespace OneVsMany
{
    [UpdateAfter(typeof(FlockingSystem))]
    [UpdateAfter(typeof(PlayerUpdateSystem))]
    public class MovementSystem : JobComponentSystem
    {        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {            
            float dt = World.Time.DeltaTime;

            JobHandle jobHandle = Entities.ForEach((ref Bullet bullet, ref Movement movement, ref Translation position, ref BoundingVolume vol) =>
            {
                if (bullet.isActive)
                {
                    // move the bullet along the direction that it was shot
                    position.Value += movement.direction * movement.speed * dt;
                    vol.volume.center = position.Value;
                    bullet.age += dt;

                    if (bullet.age >= 3)
                    {
                        bullet.isActive = false;
                        bullet.age = 0;
                    }
                }
                else
                {
                    position.Value.x = 1000;
                    movement.direction = float3.zero;
                }
            }).Schedule(inputDeps);
            jobHandle.Complete();
            return jobHandle;
        }
    }
}
