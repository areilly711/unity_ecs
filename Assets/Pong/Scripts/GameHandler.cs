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
