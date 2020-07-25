using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Memory
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class CardAuthoring : MonoBehaviour, IConvertGameObjectToEntity//, IDeclareReferencedPrefabs
    {
        public int value;
        public GameObject face;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(dstManager.World, conversionSystem.BlobAssetStore);
            //Entity faceEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(face, settings);
            Entity faceEntity = conversionSystem.GetPrimaryEntity(face);
            dstManager.AddComponentData<Card>(entity, new Card() { value = value, face = faceEntity });
        }

        //public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        //{
        //    referencedPrefabs.Add(face);
        //}
    }
}

