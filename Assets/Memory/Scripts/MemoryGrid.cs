﻿/*
The MIT License (MIT)
Copyright 2021 Adam Reilly, Call to Action Software LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using Shared;
using Unity.Collections;

namespace Memory
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class MemoryGrid : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Transform m_minPos;
        public Transform m_maxPos;

        public CardAuthoring cardGameObject;
        public int pairs = 10;
        public Material[] m_cardMaterials;

        NativeList<float3> cardPositions;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            cardPositions = new NativeList<float3>(Allocator.Temp);
            int numCards = pairs * 2;
            
            // try to balance between rows and cols based on the number of cards required
            int cols = (int)math.round(math.sqrt(numCards));
            float width = (m_maxPos.position.x - m_minPos.position.x) / (cols);
            float height = (m_maxPos.position.y - m_minPos.position.y) / (cols);

            int row = -1;
            for (int i = 0; i < numCards; i++)
            {
                if (i % cols == 0)
                {
                    row += 1;
                }

                // create card positions based on the number of columns. Offset by the min position
                float3 pos = m_minPos.position;
                pos.x += (i % cols) * width;
                pos.y += (row) * height;
                cardPositions.Add(pos);                
            }

            // create the card pairs, assigning the same value and material to each member of the pair
            for (int i = 0; i < pairs; i++)
            {
                int value = i % m_cardMaterials.Length;
                CreateCard(ExtractPosition(), value, m_cardMaterials[value], dstManager, conversionSystem);
                CreateCard(ExtractPosition(), value, m_cardMaterials[value], dstManager, conversionSystem);
            }

            cardPositions.Dispose();
        }

        float3 ExtractPosition()
        {
            int index = UnityEngine.Random.Range(0, cardPositions.Length);
            float3 pos = cardPositions[index];
            cardPositions.RemoveAt(index);
            return pos;
        }

        Entity CreateCard(float3 pos, int value, Material mat, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // extract the card from the gameobject
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(dstManager.World, conversionSystem.BlobAssetStore);
            Entity card = GameObjectConversionUtility.ConvertGameObjectHierarchy(cardGameObject.gameObject, settings);            
            
            // create the card
            card = dstManager.Instantiate(card);

            quaternion startingRot = quaternion.identity;
            
            // set the card value and face
            Card cardData = dstManager.GetComponentData<Card>(card);
            cardData.value = value;
            RenderMesh cardFace = dstManager.GetSharedComponentData<RenderMesh>(cardData.face);
            cardFace.material = mat;
            dstManager.SetSharedComponentData<RenderMesh>(cardData.face, cardFace);
            dstManager.SetComponentData<TargetRotation>(card, new TargetRotation { target = startingRot });
            dstManager.SetComponentData<Card>(card, cardData);

            // set position and bounding box
            dstManager.SetComponentData<Translation>(card, new Translation { Value = pos });
            BoundingBox box = dstManager.GetComponentData<BoundingBox>(card);
            box.aabb.Center = pos;
            dstManager.SetComponentData<BoundingBox>(card, box);
            return card;
        }
    }
}