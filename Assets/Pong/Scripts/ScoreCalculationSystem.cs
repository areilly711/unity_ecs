/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Shared;

namespace Pong
{
    public class ScoreCalculationSystem : SystemBase
    {
        GameHandler m_handler;
        protected override void OnCreate()
        {
            base.OnCreate();
            m_handler = UnityEngine.GameObject.FindObjectOfType<GameHandler>();
        }

        protected override void OnUpdate()
        {
            // get the ball's position
            EntityQuery query = EntityManager.CreateEntityQuery(typeof(Translation), typeof(Ball));
            if (query.CalculateEntityCount() != 1)
            {
                return;
            }
            float3 ballPos = EntityManager.CreateEntityQuery(typeof(Translation), typeof(Ball)).GetSingleton<Translation>().Value;

            query = EntityManager.CreateEntityQuery(typeof(Translation), typeof(Player));
            if (query.CalculateEntityCount() != 1)
            {
                return;
            }
            float3 playerPos = query.GetSingleton<Translation>().Value;

            query = EntityManager.CreateEntityQuery(typeof(Translation), typeof(AI));
            if (query.CalculateEntityCount() != 1)
            {
                return;
            }
            float3 aiPos = query.GetSingleton<Translation>().Value;

            if (ballPos.x <= playerPos.x)
            {
                // ai scored
                m_handler.PointEarned(1);
            }
            else if (ballPos.x >= aiPos.x)
            {
                // player scored
                m_handler.PointEarned(0);
            }
        }
    }
}