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
using UnityEngine;

namespace Breakout
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class BreakoutBrickBuilder : MonoBehaviour, IConvertGameObjectToEntity
    {
        public BrickAuthoring m_brick;
        public int width = 10;
        public int height = 10;
        public Transform startingPosition;        

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {            
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(dstManager.World, conversionSystem.BlobAssetStore);
            Entity brickPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_brick.gameObject, settings);
            float3 startingPos = startingPosition.position;
            CompositeScale scale = dstManager.GetComponentData<CompositeScale>(brickPrefab);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {                    
                    Entity brick = dstManager.Instantiate(brickPrefab);
                    dstManager.SetComponentData<Translation>(brick, new Translation() 
                        { Value = startingPos + new float3(i * 1.5f, j * 0.75f, 0 ) } );
                }
            }
        }
    }
}