using Unity.Entities;
using UnityEngine;

namespace Memory
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class CardAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int value;
        public GameObject face;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Entity faceEntity = conversionSystem.GetPrimaryEntity(face);
            dstManager.AddComponentData<Card>(entity, new Card() { value = value, face = faceEntity });
        }
    }
}

