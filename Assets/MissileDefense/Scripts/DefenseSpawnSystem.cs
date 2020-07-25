using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Shared;
using MissileDefense;

public class DefenseSpawnSystem : SystemBase
{
    EntityQueryDesc playerQueryDesc;
    protected override void OnCreate()
    {
        base.OnCreate();
        playerQueryDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(Player), typeof(AttackSpeed), typeof(Ready) }
        };
    }

    protected override void OnUpdate()
    {
        EntityQuery query = EntityManager.CreateEntityQuery(playerQueryDesc);
        if (query.CalculateEntityCount() == 0) return; 

        Entity player = query.GetSingletonEntity();
        Ready playerReadiness = query.GetSingleton<Ready>();        
        if (playerReadiness.value && Input.GetMouseButtonDown(0))
        {
            // the player's reload timer is up and they've clicked the mouse

            // spawn the defense entity and position it where the mouse is
            Entity defense = EntityManager.Instantiate(GamePrefabsAuthoring.Defense);
            Translation defensePos = EntityManager.GetComponentData<Translation>(defense);
            defensePos.Value = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            defensePos.Value.z = 0;
            EntityManager.SetComponentData<Translation>(defense, defensePos);
            playerReadiness.value = false;
            EntityManager.SetComponentData<Ready>(player, playerReadiness);
        }
    }
}
