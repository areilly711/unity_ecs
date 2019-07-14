using System.Collections;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace OneVsMany
{
    public class GameHandler : MonoBehaviour
    {
        const int NumBullets = 25;
        //const int MaxHealth = 100;
        public float bulletDamage = 5;
        public int numEnemies = 1;
        public float enemySpeed = 0.02f;
        public float enemyHealth = 10;
        public float enemySpawnInterval = 10;
        public float foodSpawnInterval = 20;
        public float maxPlayerHealth = 50;
        public float healthDegenRate = 1;
        public Mesh mesh;
        public Material playerMat;
        public Material enemyMat;
        public Material bulletMat;
        public Material foodMat;
        public Hud hud;

        // Start is called before the first frame update
        EntityManager entityManager;
        public static Entity playerEntity;

        void Start()
        {
            entityManager = World.Active.EntityManager;
            entityManager.World.GetOrCreateSystem<PlayerUpdateSystem>().Init(healthDegenRate, hud);
            entityManager.World.GetOrCreateSystem<PlayerUpdateSystem>().Enabled = false;
            entityManager.World.GetOrCreateSystem<FlockingSystem>().Enabled = false;
            entityManager.World.GetOrCreateSystem<FlockingSystem>().Enabled = false;
            entityManager.World.GetOrCreateSystem<CollisionSystem>().Enabled = false;
            entityManager.World.GetOrCreateSystem<MovementSystem>().Enabled = false;


            CreatePlayer();
            //CreateEnemies(numEnemies);
            CreateBullets(NumBullets);
            
        }

        public void GameOver()
        {
            // stop all the systems
            foreach (ComponentSystemBase s in entityManager.World.Systems)
            {
                s.Enabled = false;
            }
            StopAllCoroutines();
        }

        public void Restart()
        {
            foreach (ComponentSystemBase s in entityManager.World.Systems)
            {
                s.Enabled = true;
            }

            // get all food and enemies
            EntityQueryDesc desc = new EntityQueryDesc()
            {
                Any = new ComponentType[]{ typeof(Enemy), typeof(Food) }
            };
            EntityQuery q = entityManager.CreateEntityQuery(desc);
            NativeArray<Entity> entitiesToDestroy = q.ToEntityArray(Allocator.TempJob);
            for (int i = 0; i < entitiesToDestroy.Length; i++)
            {
                // delete all food and enemies
                entityManager.DestroyEntity(entitiesToDestroy[i]);
            }
            entitiesToDestroy.Dispose();

            // give the player max health again
            InitHealth(playerEntity, maxPlayerHealth, maxPlayerHealth);

            StartCoroutine(SpawnFood(foodSpawnInterval));
            StartCoroutine(SpawnEnemies(enemySpawnInterval, numEnemies));
        }

        IEnumerator SpawnFood(float interval)
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);
                CreateFood(1, entityManager.GetComponentData<Translation>(playerEntity).Value);
            }
        }

        IEnumerator SpawnEnemies(float interval, int numEnemies)
        {
            while (true)
            {
                CreateEnemies(numEnemies);
                yield return new WaitForSeconds(interval);
            }
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    CreateFood(1, entityManager.GetComponentData<Translation>(playerEntity).Value);
            //}

            //Health playerHealth = entityManager.GetComponentData<Health>(playerEntity);
            //hud.SetHealth(playerHealth.curr);

            //if (playerHealth.curr <= 0)
            //{
            //    // game over
            //}
        }

        private void LateUpdate()
        {
            //Camera.main.transform.LookAt(entityManager.GetComponentData<Translation>(playerEntity).Value);
        }

        void CreatePlayer()
        {
            playerEntity = entityManager.CreateEntity(
               typeof(Movement),
               typeof(Translation),
               typeof(LocalToWorld),
               typeof(RenderMesh),
               typeof(Scale),
               typeof(BoundingVolume),
               typeof(Health),
               typeof(Player)
           );

            entityManager.SetComponentData<Movement>(playerEntity, new Movement { speed = 5 });
            
            hud.SetMaxHealth(maxPlayerHealth);
            InitHealth(playerEntity, maxPlayerHealth, maxPlayerHealth);
            InitRenderData(playerEntity, new float3(0), 1, mesh, playerMat);
        }

        void CreateEnemies(int numEnemies)
        {
            for (int i = 0; i < numEnemies; i++)
            {
                Entity e = entityManager.CreateEntity(
                    typeof(Movement),
                    typeof(Translation),
                    typeof(LocalToWorld),
                    typeof(RenderMesh),
                    typeof(NonUniformScale),
                    typeof(BoundingVolume),
                    typeof(Health),
                    typeof(HealthModifier),
                    typeof(Enemy)
                );

                entityManager.SetComponentData<Movement>(e, new Movement { speed = enemySpeed });
                entityManager.SetComponentData<Enemy>(e, new Enemy { points = (int)enemyHealth });
                InitHealth(e, enemyHealth, enemyHealth);
                InitHealthModifier(e, -10/*MaxHealth*/);
                InitRenderData(e, CreateRandomSpawnPosition(Vector2.zero, 15, 20), 0.5f, mesh, enemyMat);
            }
        }

        Vector3 CreateRandomSpawnPosition(Vector3 center, float minR, float maxR)
        {
            Vector2 spawnPos = UnityEngine.Random.insideUnitCircle * minR;
            spawnPos += UnityEngine.Random.insideUnitCircle * maxR;
            Vector3 final = spawnPos;
            final += center;
            return spawnPos;
        }

        void CreateBullets(int numBullets)
        {
            for (int i = 0; i < numBullets; i++)
            {
                Entity e = entityManager.CreateEntity(
                    typeof(Movement),
                    typeof(Translation),
                    typeof(LocalToWorld),
                    typeof(RenderMesh),
                    typeof(Scale),
                    typeof(BoundingVolume),
                    typeof(HealthModifier),
                    typeof(Bullet)
                );

                InitHealthModifier(e, -bulletDamage);
                entityManager.SetComponentData<Movement>(e, new Movement { speed = 0 });
                InitRenderData(e, new float3(1000, 0, 0), 0.1f, mesh, bulletMat);
            }
        }

        void CreateFood(int numFood, float3 playerPos)
        {
            for (int i = 0; i < numFood; i++)
            {
                Entity e = entityManager.CreateEntity(
                    typeof(Translation),
                    typeof(LocalToWorld),
                    typeof(RenderMesh),
                    typeof(Scale),
                    typeof(BoundingVolume),
                    typeof(HealthModifier),
                    typeof(Food)
                );

                InitHealthModifier(e, maxPlayerHealth);
                InitRenderData(e, CreateRandomSpawnPosition(playerPos, 2 , 2), 0.4f, mesh, foodMat);
            }
        }

        void InitHealth(Entity e, float curr, float max)
        {
            entityManager.SetComponentData<Health>(e, new Health { curr = curr, max = max });
        }

        void InitHealthModifier(Entity e, float amount)
        {
            entityManager.SetComponentData<HealthModifier>(e, new HealthModifier { value = amount });
        }

        void InitRenderData(Entity e, float3 pos, float scale, Mesh mesh, Material mat)
        {
            if (entityManager.HasComponent<Scale>(e))
            {
                entityManager.SetComponentData<Scale>(e, new Scale { Value = scale });
            }
            else if (entityManager.HasComponent<NonUniformScale>(e))
            {
                entityManager.SetComponentData<NonUniformScale>(e, new NonUniformScale { Value = new float3(scale, scale, scale) });
            }
            entityManager.SetComponentData<Translation>(e, new Translation { Value = pos });
            entityManager.SetSharedComponentData<RenderMesh>(e, new RenderMesh { mesh = mesh, material = mat });

            Bounds b = new Bounds();
            b.center = pos;
            float halfScale = scale * 0.5f;
            b.extents = new float3(halfScale, halfScale, halfScale);
            entityManager.SetComponentData<BoundingVolume>(e, new BoundingVolume { volume = b });
        }
    }
}
