using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public struct ObstacleData
    {
        public Vector3 pos;
        public float radius;
        public bool onRoad;
    }

    /// <summary>
    /// Keeps gameplay rules in one place for both the game and the level validator,
    /// ensuring that validation simulates the same rules used during play.
    /// </summary>
    public static class LevelRules
    {
        public const float InfectMultiplier = 5f; // Infection radius = shot radius * this value.
        public const float ChainDecay = 0.6f; // Each generation has a smaller infection radius.
        public const float MinChainRadius = 0.5f; // Stop the chain below this radius.
        public const float CriticalRadius = 0.3f; // Falling below this radius causes a loss.
        public const float MinShotRadius = 0.15f; // Minimum shot radius.
        public const float PlayerGap = 0.15f; // Clearance between the player and an obstacle.

        public static float VolToRadius(float v) => Mathf.Pow(Mathf.Max(v, 0f), 1f / 3f);
        public static float RadiusToVol(float r) => r * r * r;

        /// <summary>Finds the first obstacle hit by a shot traveling along +Z.</summary>
        public static int FindFirstHit(IList<ObstacleData> obs, bool[] dead, Vector3 origin,
            float shotR, out Vector3 hitPoint)
        {
            int best = -1;
            float bestZ = float.MaxValue;
            hitPoint = origin;
            for (int i = 0; i < obs.Count; i++)
            {
                if (dead[i]) continue;
                ObstacleData o = obs[i];
                if (o.pos.z < origin.z) continue;
                float reach = o.radius + shotR;
                float lat = Mathf.Abs(o.pos.x - origin.x);
                if (lat > reach) continue;
                float hz = o.pos.z - Mathf.Sqrt(reach * reach - lat * lat);
                if (hz < bestZ)
                {
                    bestZ = hz;
                    best = i;
                }
            }

            if (best >= 0) hitPoint = new Vector3(origin.x, origin.y, Mathf.Max(bestZ, origin.z));
            return best;
        }

        /// <summary>Finds the nearest obstacle blocking the player at its current radius.</summary>
        public static int FindBlocker(IList<ObstacleData> obs, bool[] dead, float playerZ,
            float playerR, out float stopZ)
        {
            int best = -1;
            stopZ = float.MaxValue;
            for (int i = 0; i < obs.Count; i++)
            {
                if (dead[i]) continue;
                ObstacleData o = obs[i];
                if (o.pos.z < playerZ) continue;
                float reach = o.radius + playerR + PlayerGap;
                float lat = Mathf.Abs(o.pos.x); // The player stays centered at x = 0.
                if (lat >= reach) continue;
                float sz = o.pos.z - Mathf.Sqrt(reach * reach - lat * lat);
                if (sz < stopZ)
                {
                    stopZ = sz;
                    best = i;
                }
            }

            return best;
        }

        /// <summary>
        /// Infects the first obstacle and nearby obstacles, then propagates a decaying chain reaction.
        /// Obstacles closer together are more likely to be infected by the chain.
        /// Marks infected obstacles in dead[] and returns their indices and chain depths for explosion timing.
        /// </summary>
        public static void Infect(IList<ObstacleData> obs, bool[] dead, int hit, float shotR,
            List<int> outIdx, List<int> outDepth)
        {
            float baseR = shotR * InfectMultiplier;
            outIdx.Add(hit);
            outDepth.Add(0);
            dead[hit] = true;

            for (int k = 0; k < outIdx.Count; k++)
            {
                Vector3 src = obs[outIdx[k]].pos;
                float r = baseR * Mathf.Pow(ChainDecay, outDepth[k]);
                if (r < MinChainRadius) continue;
                for (int i = 0; i < obs.Count; i++)
                {
                    if (dead[i]) continue;
                    Vector2 d = new Vector2(obs[i].pos.x - src.x, obs[i].pos.z - src.z);
                    if (d.magnitude <= r + obs[i].radius)
                    {
                        dead[i] = true;
                        outIdx.Add(i);
                        outDepth.Add(outDepth[k] + 1);
                    }
                }
            }
        }
    }
}
