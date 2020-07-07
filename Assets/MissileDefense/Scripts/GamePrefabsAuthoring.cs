using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MissileDefense
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class GamePrefabsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject m_missileGameObject;
        public static Entity Missile;

        public GameObject m_defenseGameObject;
        public static Entity Defense;

        public GameObject m_buildingGameObject;
        public static Entity Building;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            using (BlobAssetStore store = new BlobAssetStore())
            {
                Missile = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_missileGameObject,
                    GameObjectConversionSettings.FromWorld(dstManager.World, store));

                Defense = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_defenseGameObject,
                    GameObjectConversionSettings.FromWorld(dstManager.World, store));

                Building = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_buildingGameObject,
                    GameObjectConversionSettings.FromWorld(dstManager.World, store));
            }
        }
    }
}