/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Shared;

namespace MissileDefense
{
    public class MissileSpawnSystem : SystemBase
    {
        float m_spawnTimer;
        Random m_random;
        float3 forward;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_random = new Random();
            m_random.InitState();
            forward = new float3(0, 1, 0);
        }

        protected override void OnUpdate()
        {
            EntityQuery query = EntityManager.CreateEntityQuery(typeof(GameSettings));
            if (query.CalculateEntityCount() == 0) return;
            
            GameSettings settings = EntityManager.CreateEntityQuery(typeof(GameSettings)).GetSingleton<GameSettings>();
            
            if (m_spawnTimer >= settings.spawnRate)
            {
                EntityQuery buildingQuery = EntityManager.CreateEntityQuery(typeof(Building), typeof(Translation));
                if (buildingQuery.CalculateEntityCount() == 0)
                {
                    // all buildings have been destroyed, game over
                    return;
                }

                NativeArray<Translation> buildingPositions = buildingQuery.ToComponentDataArray<Translation>(Allocator.TempJob);                
                for (int i = 0; i < settings.spawns; i++)
                {
                    Entity missile = EntityManager.Instantiate(GamePrefabsAuthoring.Missile);
                    // randomize x pos
                    Translation pos = EntityManager.GetComponentData<Translation>(missile);
                    pos.Value.x = m_random.NextFloat(settings.posMin, settings.posMax);
                    pos.Value.y = settings.spawnYPos;
                    pos.Value.z = 0;
                    EntityManager.SetComponentData<Translation>(missile, pos);

                    // randomize speed
                    Speed s = EntityManager.GetComponentData<Speed>(missile);
                    s.value = m_random.NextFloat(settings.speedMin, settings.speedMax);
                    EntityManager.SetComponentData<Speed>(missile, s);

                    // move the missile towards a random building
                    Rotation rot = EntityManager.GetComponentData<Rotation>(missile);
                    Translation target = buildingPositions[m_random.NextInt(buildingPositions.Length)];
                    float3 dir = math.normalize(target.Value - pos.Value);
                    EntityManager.SetComponentData<Direction>(missile, new Direction { value = dir });
                    //float z = math.degrees(math.atan2(dir.y, dir.x));
                    //rot.Value = quaternion.Euler(0, 0, z);                    
                    //rot.Value = quaternion.EulerZYX(0, 0, z);
                    //rot.Value = quaternion.LookRotationSafe(target.Value - pos.Value, forward);*/
                    //EntityManager.SetComponentData<Rotation>(missile, rot);


                    //LocalToWorld ltw = EntityManager.GetComponentData<LocalToWorld>(missile);
                    //ltw.Value = float4x4.EulerXYZ(0, 0, z);
                    //EntityManager.SetComponentData<LocalToWorld>(missile, ltw);

                }
                m_spawnTimer = 0;
                buildingPositions.Dispose();
            }
            m_spawnTimer += Time.DeltaTime;
        }
    }
}