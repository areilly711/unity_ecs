using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using System.Collections.Generic;
using System.Data;
using Shared;

namespace Memory
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class MatchingGrid : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Transform m_minPos;
        public Transform m_maxPos;

        public CardAuthoring cardGameObject;
        public int pairs = 10;
        public Material[] m_cardMaterials;

        List<float3> cardPositions = new List<float3>();

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            int numCards = pairs * 2;
            float2 cardSize = new float2(2f, 2f);

            int cols = (int)math.round(math.sqrt(numCards));
            float width = (m_maxPos.position.x - m_minPos.position.x) / (cols);
            float height = (m_maxPos.position.y - m_minPos.position.y) / (cols);

            /*for (int i = 0; i < pairs; i++)
            {
                for (int j = 0; j < pairs; j++)
                {
                    float3 pos = m_minPos.position;
                    pos.x += i * width;
                    pos.y += j * height;
                    cardPositions.Add(pos);
                }
            }*/

            int row = -1;
            for (int i = 0; i < numCards; i++)
            {
                if (i % cols == 0)
                {
                    row += 1;
                }

                float3 pos = m_minPos.position;
                pos.x += (i % cols) * width;
                pos.y += (row) * height;
                cardPositions.Add(pos);
                
            }

            for (int i = 0; i < pairs; i++)
            {                
                //pos.x = m_minPos.position.x + cardSize.x * i;
                //pos.y = m_minPos.position.y + cardSize.y * i;

                int value = i % m_cardMaterials.Length;
                CreateCard(ExtractPosition(), value, m_cardMaterials[value], dstManager, conversionSystem);
                CreateCard(ExtractPosition(), value, m_cardMaterials[value], dstManager, conversionSystem);
            }
        }

        float3 ExtractPosition()
        {
            int index = UnityEngine.Random.Range(0, cardPositions.Count);
            float3 pos = cardPositions[index];
            cardPositions.RemoveAt(index);
            return pos;
        }

        Entity CreateCard(float3 pos, int value, Material mat, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(dstManager.World, conversionSystem.BlobAssetStore);
            Entity card = GameObjectConversionUtility.ConvertGameObjectHierarchy(cardGameObject.gameObject, settings);            
            
            card = dstManager.Instantiate(card);
            quaternion startingRot = quaternion.identity;// quaternion.Euler(0, math.radians(180), 0);
            dstManager.SetComponentData<Rotation>(card, new Rotation() { Value = startingRot });
            Card cardData = dstManager.GetComponentData<Card>(card);
            cardData.value = value;
            RenderMesh cardFace = dstManager.GetSharedComponentData<RenderMesh>(cardData.face);
            cardFace.material = mat;
            dstManager.SetSharedComponentData<RenderMesh>(cardData.face, cardFace);
            dstManager.SetComponentData<TargetRotation>(card, new TargetRotation { target = startingRot });
            dstManager.SetComponentData<Card>(card, cardData);
            dstManager.SetComponentData<Translation>(card, new Translation { Value = pos });
            BoundingBox box = dstManager.GetComponentData<BoundingBox>(card);
            box.aabb.Center = pos;
            dstManager.SetComponentData<BoundingBox>(card, box);
            return card;
        }
    }
}