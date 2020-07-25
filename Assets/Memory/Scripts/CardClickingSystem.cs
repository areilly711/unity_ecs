using Shared;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Memory
{
    public class CardClickingSystem : SystemBase
    {
        Entity[] selections;
        bool doComparison;
        float timer;
        GameHandler m_game;

        protected override void OnCreate()
        {
            base.OnCreate();
            selections = new Entity[2];
            m_game = GameObject.FindObjectOfType<GameHandler>();
        }

        protected override void OnUpdate()
        {
            Entity settingsEntity = EntityManager.CreateEntityQuery(typeof(GameSettings)).GetSingletonEntity();
            GameSettings settings = EntityManager.GetComponentData<GameSettings>(settingsEntity);


            float dt = World.Time.DeltaTime;
            if (doComparison)
            {
                timer -= dt;
                if (timer <= 0)
                {
                    doComparison = false;
                    settings.facesShowing = 0;
                    EntityManager.SetComponentData<GameSettings>(settingsEntity, settings);

                    // comparison time
                    ComponentDataFromEntity<Card> cards = GetComponentDataFromEntity<Card>(true);
                    int value = cards[selections[0]].value;
                    bool cardsMatch = true;
                    for (int i = 1; i < selections.Length; i++)
                    {
                        if (cards[selections[i]].value != value)
                        {
                            cardsMatch = false;
                            break;
                        }
                    }

                    if (cardsMatch)
                    {
                        // they match, remove the matches
                        for (int i = 0; i < selections.Length; i++)
                        {
                            EntityManager.DestroyEntity(selections[i]);
                        }                        
                    }
                    else
                    {
                        // they don't match, flip them back over
                        for (int i = 0; i < selections.Length; i++)
                        {
                            EntityManager.SetComponentData<TargetRotation>(selections[i],
                                new TargetRotation { target = quaternion.identity });
                            EntityManager.SetComponentData<Timer>(selections[i],
                                new Timer { curr = 0, max = settings.secondsToTurn });
                        }
                    }

                    // check if all the cards have been matched
                    if (EntityManager.CreateEntityQuery(typeof(Card)).CalculateEntityCount() == 0)
                    {
                        m_game.ResetGame();
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (settings.facesShowing >= 2)
                {
                    return;
                }

                float3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                Entity clickedCard = Entity.Null;
                Entities.WithAll<Card>().ForEach((Entity cardEnt, in BoundingBox box) =>
                {                    
                    if (box.aabb.Contains(pos))
                    {
                        clickedCard = cardEnt;                        
                    }
                }).Run();

                if (clickedCard != Entity.Null)
                {
                    selections[settings.facesShowing] = clickedCard;
                    settings.facesShowing += 1;
                    EntityManager.SetComponentData<TargetRotation>(clickedCard, new TargetRotation { target = quaternion.Euler(0, math.radians(180), 0) });
                    EntityManager.SetComponentData<Timer>(clickedCard, new Timer { curr = 0, max = settings.secondsToTurn });
                    EntityManager.SetComponentData<GameSettings>(settingsEntity, settings);
                    if (settings.facesShowing == 2)
                    {
                        timer = settings.secondsToTurn;
                        doComparison = true;
                    }
                }
            }
        }
    }
}