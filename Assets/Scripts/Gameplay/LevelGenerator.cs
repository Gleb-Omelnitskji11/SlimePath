using System.Collections.Generic;
using Core.ServiceLocator;
using UnityEngine;

namespace Gameplay
{
    public class LevelGenerator : MonoBehaviour, IService
    {
        [Header("Level")]
        [SerializeField] private  int seed = 12345;
        [SerializeField] private  float goalZ = 13f;
        [SerializeField] private  float startClearZone = 2f;
        [SerializeField] private  float doorClearZone = 1f;
        [SerializeField] private  float startRadius = 1f;

        [Header("Road Obstacles")]
        [SerializeField] private  float roadObstacleMaxX = 1.6f; // Maximum absolute x offset for obstacles on the road.
        [SerializeField] private  Vector2 obstacleRadius = new Vector2(0.3f, 0.5f);
        [SerializeField] private  Vector2 spacing = new Vector2(1.5f, 3.2f);
        [Range(0, 1)] [SerializeField] private  float roadDensity = 0.8f;
        [Range(0, 1)] [SerializeField] private  float clusterChance = 0.4f;

        [Header("Side Obstacles")]
        [SerializeField] private  float sideMinX = 2.2f;
        [SerializeField] private  float sideMaxX = 4f;
        [Range(0, 1)] public float sideDensity = 0.7f;

        [Header("Validation")]
        [SerializeField] private  int maxAttempts = 50;
        [SerializeField] private  float playerWasteFactor = 1.2f; // Extra allowance for player input imprecision.
        
        [Header("Materials")]
        [SerializeField] private Material _obstacleMaterial;
        [SerializeField] private Material _obstacleExplodedMaterial;
        [SerializeField] private Material _roadMaterial;
        [SerializeField] private Material _doorMaterial;

        public List<ObstacleData> Data { get; private set; }
        public bool[] Dead { get; private set; }
        public float GoalZ => goalZ;
        public GateController Gate { get; private set; }
        public float StartRadius => startRadius;
        public float DoorClearZone => doorClearZone;

        private Obstacle[] _objs;
        private Transform _road;

        public void Build()
        {
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Random.InitState(seed + attempt);
                // Reduce obstacle density after each failed attempt.
                float k = 1f - attempt * 0.02f;
                var data = Generate(roadDensity * k);
                if (IsSolvable(data))
                {
                    Data = data;
                    Debug.Log($"Level OK, attempt {attempt}, obstacles {data.Count}");
                    Spawn();
                    return;
                }
                
                Debug.LogWarning($"Level generation attempt {attempt} failed.");
            }
            
            Debug.LogWarning("All level generation attempts failed; generating a minimal level.");
            Random.InitState(seed);
            Data = Generate(0.15f);
            Spawn();
        }

        // ---------- Level data generating ----------
        private List<ObstacleData> Generate(float density)
        {
            var list = new List<ObstacleData>();
            float z = startClearZone;
            while (z < goalZ - doorClearZone)
            {
                if (Random.value < density)
                {
                    int n = Random.value < clusterChance ? Random.Range(2, 5) : 1;
                    float cx = Random.Range(-roadObstacleMaxX, roadObstacleMaxX);
                    for (int i = 0; i < n; i++)
                    {
                        float r = Random.Range(obstacleRadius.x, obstacleRadius.y);
                        float x = n > 1 ? cx + Random.Range(-0.9f, 0.9f) : cx;
                        x = Mathf.Clamp(x, -roadObstacleMaxX, roadObstacleMaxX);
                        list.Add(new ObstacleData
                        {
                            pos = new Vector3(x, r, z + Random.Range(-0.6f, 0.6f)),
                            radius = r, onRoad = true
                        });
                    }
                }

                // Generate obstacles alongside the road.
                for (int side = -1; side <= 1; side += 2)
                {
                    if (Random.value < sideDensity)
                    {
                        float r = Random.Range(obstacleRadius.x, obstacleRadius.y);
                        list.Add(new ObstacleData
                        {
                            pos = new Vector3(side * Random.Range(sideMinX, sideMaxX), r,
                                z + Random.Range(-1f, 1f)),
                            radius = r, onRoad = false
                        });
                    }
                }

                z += Random.Range(spacing.x, spacing.y);
            }

            return list;
        }

        // ---------- Solvability validation ----------
        /// <summary>
        /// Simulates a greedy player who fires the smallest sufficient shot at each blocking obstacle.
        /// The level is solvable if the player has enough volume to reach the goal.
        /// Shot costs include playerWasteFactor so the level remains practical for a human player.
        /// </summary>
        private bool IsSolvable(List<ObstacleData> data)
        {
            bool[] dead = new bool[data.Count];
            float playerVol = LevelRules.RadiusToVol(startRadius);
            float minVol = LevelRules.RadiusToVol(LevelRules.CriticalRadius * 1.2f);
            float z = 0f;
            var idx = new List<int>();
            var dep = new List<int>();

            for (int guard = 0; guard < 1000; guard++)
            {
                float pr = LevelRules.VolToRadius(playerVol);
                int blocker = LevelRules.FindBlocker(data, dead, z, pr, out float stopZ);
                if (blocker < 0) return true;
                z = Mathf.Max(z, stopZ);

                bool cleared = false;
                for (float sr = LevelRules.MinShotRadius; sr < 3f; sr += 0.05f)
                {
                    float cost = LevelRules.RadiusToVol(sr * playerWasteFactor);
                    if (playerVol - cost < minVol) break; // Not enough player volume remains.

                    var tmp = (bool[])dead.Clone();
                    int hit = LevelRules.FindFirstHit(data, tmp, new Vector3(0, 0, z), sr, out _);
                    if (hit < 0) break;
                    idx.Clear();
                    dep.Clear();
                    LevelRules.Infect(data, tmp, hit, sr, idx, dep);
                    if (tmp[blocker])
                    {
                        dead = tmp;
                        playerVol -= cost;
                        cleared = true;
                        break;
                    }
                }

                if (!cleared) return false;
            }

            return false;
        }

        // ---------- Object creation ----------
        private void Spawn()
        {
            Dead = new bool[Data.Count];
            _objs = new Obstacle[Data.Count];

            // Obstacles
            for (int i = 0; i < Data.Count; i++)
            {
                var d = Data[i];
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(go.GetComponent<Collider>());
                go.transform.SetParent(transform);
                go.transform.position = d.pos;
                go.transform.localScale = Vector3.one * d.radius * 2f;
                go.GetComponent<Renderer>().sharedMaterial  = _obstacleMaterial;
                var ob = go.AddComponent<Obstacle>();
                ob.radius = d.radius;
                _objs[i] = ob;
            }

            // Road
            var r = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(r.GetComponent<Collider>());
            r.transform.SetParent(transform);
            r.name = "Road";
            r.GetComponent<Renderer>().material = _roadMaterial;
            _road = r.transform;
            SetRoad(startRadius * 2f);

            // Double gate with leaves hinged at the outer edges.
            Gate = GateController.Create(transform, goalZ, startRadius, _doorMaterial);
        }

        public void SetRoad(float halfWidth)
        {
            if (_road == null) return;
            _road.position = new Vector3(0, 0.05f, goalZ * 0.5f);
            _road.localScale = new Vector3(halfWidth * 2f, 0.1f, goalZ + 4f);
        }

        public void InfectFrom(int hit, float shotR)
        {
            var idx = new List<int>();
            var dep = new List<int>();
            LevelRules.Infect(Data, Dead, hit, shotR, idx, dep);
            for (int i = 0; i < idx.Count; i++)
                _objs[idx[i]].Explode(dep[i] * 0.12f, _obstacleExplodedMaterial);
        }

    }
}
