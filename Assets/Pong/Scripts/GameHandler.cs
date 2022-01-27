/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using Unity.Entities;
using TMPro;
using Unity.Physics;
using Unity.Transforms;
using Shared;
using Unity.Mathematics;

namespace Pong
{
    public class GameHandler : MonoBehaviour
    {
        public TextMeshProUGUI[] m_scoreTexts;
        int[] m_scores;
        Unity.Mathematics.Random r = new Unity.Mathematics.Random();

        private void Start()
        {
            m_scores = new int[2];
            r.InitState();
            ResetBall();
        }

        public void PointEarned(int scorer)
        {
            m_scores[scorer] += 1;
            m_scoreTexts[scorer].text = m_scores[scorer].ToString();
            ResetBall();
        }

        void ResetBall()
        {
            // reset ball position
            EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity ball = em.CreateEntityQuery(typeof(Translation), typeof(Ball), typeof(PhysicsVelocity)).GetSingletonEntity();
            em.SetComponentData<Translation>(ball, new Translation { Value = float3.zero });
            float x = r.NextFloat(1.5f, 2);
            float y = r.NextFloat(2, 3);
            x *= r.NextInt() % 2 == 0 ? 1 : -1;
            y *= r.NextInt() % 2 == 0 ? 1 : -1;
            //y = 0;
            em.SetComponentData<PhysicsVelocity>(ball, new PhysicsVelocity() { Linear = new float3(x, y, 0) });
        }
    }
}
