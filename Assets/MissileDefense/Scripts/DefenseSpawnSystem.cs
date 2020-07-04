using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
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
        //playerQuery = EntityManager.CreateEntityQuery(typeof(Player), typeof(AttackSpeed));
    }

    protected override void OnUpdate()
    {
        //float3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        EntityQuery query = EntityManager.CreateEntityQuery(playerQueryDesc);
        Entity player = query.GetSingletonEntity();
        Ready playerReadiness = query.GetSingleton<Ready>();        
        if (playerReadiness.value && Input.GetMouseButtonDown(0))
        {
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
