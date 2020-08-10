using UnityEngine;
using Unity.Entities;

namespace Breakout
{
    public class BrickAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Brick>(entity);
        }
    }
}
