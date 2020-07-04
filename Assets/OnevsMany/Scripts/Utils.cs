using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Shared;

namespace OneVsMany
{
    static public class Utils
    {
        public static void ModifyHealth(ref HealthFloat health, float mod)
        {
            health.curr += mod;
            health.curr = math.clamp(health.curr, 0, health.max);
        }

        public static float3 Seek(float3 target, float3 start, float speed)
        {
            return math.normalizesafe((target - start)) * speed;
        }

        // Separation
        // Method checks for nearby boids and steers away
        // From https://processing.org/examples/flocking.html
        public static float3 Separate(float3 position, float3 velocity, [ReadOnly] NativeArray<Translation> boids, float desiredSeparation, float maxSpeed, float maxForce)
        {
            float3 steer = float3.zero;
            int count = 0;

            // For every boid in the system, check if it's too close
            for (int i = 0; i < boids.Length; i++)
            {
                Translation b = boids[i];
                float d = math.distance(position, b.Value);
                // If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
                if ((d > 0) && (d < desiredSeparation))
                {
                    // Calculate vector pointing away from neighbor
                    float3 diff = position - b.Value;
                    diff = math.normalizesafe(diff);
                    diff /= d;        // Weight by distance
                    steer += diff;
                    count++;            // Keep track of how many
                }
            }
            // Average -- divide by how many
            if (count > 0)
            {
                steer /= count;
            }

            // As long as the vector is greater than 0
            if (math.length(steer) > 0)
            {
                // Implement Reynolds: Steering = Desired - Velocity
                steer = math.normalizesafe(steer);
                steer *= maxSpeed;
                steer -= velocity;
                steer = Clamp(steer, maxForce);
            }
            return steer;
        }


        // Alignment
        // For every nearby boid in the system, calculate the average velocity
        // From https://processing.org/examples/flocking.html
        public static float3 Align(float3 position, float3 velocity, [ReadOnly] NativeArray<Translation> boidPositions, [ReadOnly] NativeArray<Movement> boidVelocities, float neighborDistance, float maxSpeed, float maxForce)
        {
            float3 sum = float3.zero;
            float3 steer = float3.zero;
            int count = 0;
            for (int i = 0; i < boidVelocities.Length; i++)
            {
                float3 boidVelocity = boidVelocities[i].direction;
                float d = math.distance(position, boidPositions[i].Value);
                if ((d > 0) && (d < neighborDistance))
                {
                    sum += boidVelocity;
                    count++;
                }
            }
            if (count > 0)
            {
                sum /= count;

                // Implement Reynolds: Steering = Desired - Velocity
                sum = math.normalizesafe(sum);
                sum *= maxSpeed;
                steer = sum - velocity;
                steer = Clamp(steer, maxForce);               
            }
            return steer;
        }

        // Cohesion
        // For the average position (i.e. center) of all nearby boids, calculate steering vector towards that position
        // From https://processing.org/examples/flocking.html
        public static float3 Cohesion(float3 position, float3 velocity, [ReadOnly] NativeArray<Translation> boids, float neighborDistance, float maxSpeed)
        {
            float3 sum = float3.zero;   // Start with empty vector to accumulate all positions
            int count = 0;
            for (int i = 0; i < boids.Length; i++)
            {
                Translation b = boids[i];
                float d = math.distance(position, b.Value);
                if ((d > 0) && (d < neighborDistance))
                {
                    sum += b.Value; // Add position
                    count++;
                }
            }
            if (count > 0)
            {
                sum /= count;
                return Seek(sum, position, maxSpeed);  // Steer towards the position
            }
            else
            {
                return float3.zero;
            }
        }

        public static float3 Clamp(float3 v, float max)
        {
            float3 clamped = v;
            float mag = math.length(v);
            if (mag == 0) return v;

            float r = math.min(mag, max) / mag;
            clamped.x = v.x * r;
            clamped.y = v.y * r;
            clamped.z = v.z * r;
            return clamped;
        }
    }
}
