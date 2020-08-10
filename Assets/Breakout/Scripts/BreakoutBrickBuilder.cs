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