using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.SceneManagement;

namespace GameLife
{
    /// <summary>
    /// Create cell entities based on the settings chosen in the UI. Application entry point
    /// </summary>
    public class GameHandler : MonoBehaviour
    {
        public const int MaxSize = 1000;
        
        [SerializeField] int gridSize = 100;
        [SerializeField] Mesh cellMesh = null;
        [SerializeField] Material liveCellMaterial = null;
        [SerializeField] Transform gridMin = null;
        [SerializeField] Transform gridMax = null;
        
        EntityManager entityManager;

        public int GridSize { get { return gridSize; } }

        // Start is called before the first frame update
        void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        
        public void SetGridSize(int width)
        {
            gridSize = Mathf.Min(width, MaxSize);
        }
        
        public void StartGame()
        {
            InitSystems();
        }

        private void InitSystems()
        {
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PointInCellAABSystem>().Enabled = false;
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<LifeVerificationSystem>().Enabled = true;
        }

        public void RestartGame()
        {
            // destroy all entities
            NativeArray<Entity> allEntities = entityManager.GetAllEntities(Allocator.Temp);
            entityManager.DestroyEntity(allEntities);
            allEntities.Dispose();

            World.DefaultGameObjectInjectionWorld.GetExistingSystem<PointInCellAABSystem>().Enabled = true;
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<LifeVerificationSystem>().Enabled = false;
            
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void NextCycle()
        {
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<LifeVerificationSystem>().forceJob = true;
        }

        public void CreateGrid()
        {
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<LifeVerificationSystem>().Enabled = false;
            
            int gridWidth = gridSize;
            int gridHeight = gridSize;

            float scaleX = Mathf.Abs((gridMax.position.x - gridMin.position.x) / gridWidth);
            float scaleY = Mathf.Abs((gridMax.position.y - gridMin.position.y) / gridHeight);
            float scale = Mathf.Abs(Mathf.Min(scaleX, scaleY));

            int minIndex = int.MaxValue;
            // create all the cell entities based on the grid size
            Entity[,] grid = new Entity[gridHeight, gridWidth];
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    grid[j,i] = CreateCell(i, j, scale);

                    if (minIndex > grid[j,i].Index)
                    {
                        minIndex = grid[j,i].Index;
                    }
                }
            }

            // connect cell entities as neighbors
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    Neighbors neighbors = entityManager.GetComponentData<Neighbors>(grid[i, j]);

                    if (i > 0)
                    {
                        neighbors.w = grid[i - 1, j];
                    }
                    if (j > 0)
                    {
                        neighbors.s = grid[i, j - 1];
                    }
                    if (i < gridWidth - 1)
                    {
                        neighbors.e = grid[i + 1, j];
                    }
                    if (j < gridHeight - 1)
                    {
                        neighbors.n = grid[i, j + 1];
                    }

                    if (neighbors.w != Entity.Null && neighbors.n != Entity.Null)
                    {
                        neighbors.nw = grid[i - 1, j + 1];
                    }
                    if (neighbors.w != Entity.Null && neighbors.s != Entity.Null)
                    {
                        neighbors.sw = grid[i - 1, j - 1];
                    }
                    if (neighbors.e != Entity.Null && neighbors.n != Entity.Null)
                    {
                        neighbors.ne = grid[i + 1, j + 1];
                    }
                    if (neighbors.e != Entity.Null && neighbors.s != Entity.Null)
                    {
                        neighbors.se = grid[i + 1, j - 1];
                    }

                    entityManager.SetComponentData<Neighbors>(grid[i, j], neighbors);
                }
            }

            //World.Active.GetExistingSystem<LifeVerificationSystem>().firstCellOffset = minIndex;// grid[0,0].Index;

            CreateScaleConstants(scale);
        }

        void CreateScaleConstants(float scale)
        {
            Entity invis = entityManager.CreateEntity(
                typeof (Scale),
                typeof (ScaleConst)
                );

            entityManager.SetComponentData<Scale>(invis, new Scale { Value = 0 });

            Entity vis = entityManager.CreateEntity(
                typeof(Scale),
                typeof(ScaleConst)
                );
            entityManager.SetComponentData<Scale>(vis, new Scale { Value = scale });  
        }

        Entity CreateCell(int row, int col, float scale)
        {
            Entity cell = entityManager.CreateEntity(
                typeof(Translation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(Scale),
                typeof(LifeStatus),
                typeof(LifeStatusNextCycle),
                typeof(Neighbors),
                typeof(BoundingBox),
                typeof(ClickStatus)
            );


            //entityManager.SetComponentData(cell, );
            SetCellComponentData(cell, row, col, scale);
            return cell;
        }

        void SetCellComponentData(Entity cell, int row, int col, float scale)
        {
#if UNITY_EDITOR
            entityManager.SetName(cell, string.Format("[{0},{1}]", row, col));
#endif
            entityManager.SetSharedComponentData<RenderMesh>(cell, new RenderMesh { mesh = cellMesh, material = liveCellMaterial });

            float halfScale = scale * 0.5f;

            Vector3 minPos = gridMin.transform.position;
            float3 position = new Vector3(minPos.x + halfScale + (col * scale), minPos.y + halfScale + (row * scale), 0);

            entityManager.SetComponentData<Translation>(cell,
                new Translation
                {
                    Value = position
                });

            entityManager.SetComponentData<Scale>(cell,
                new Scale
                {
                    Value = 0// invisible by default
                });

            Bounds box = new Bounds();
            box.center = position;
            box.extents = new float3(halfScale, halfScale, 1);
            entityManager.SetComponentData<BoundingBox>(cell, new BoundingBox { box = box });
            entityManager.SetComponentData<Neighbors>(cell, new Neighbors());
            entityManager.AddComponent<WorldRenderBounds>(cell);
            entityManager.AddComponent<RenderBounds>(cell);
            entityManager.AddComponent<ChunkWorldRenderBounds>(cell);
        }
    }
}
