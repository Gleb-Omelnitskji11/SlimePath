using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Config
{
    public abstract class BaseConfigScript : ScriptableObject
    {
        public abstract ConfigType Type { get; }
        public string ConfigPath => Type.ToString();

        public static async UniTask<BaseConfigScript> LoadAsync(ConfigType type)
        {
            if (!Enum.IsDefined(typeof(ConfigType), type))
            {
                Debug.LogError($"[Config] Unknown config type '{type}'.");
                return null;
            }

            string path = type.ToString();
            ResourceRequest request = Resources.LoadAsync<BaseConfigScript>(path);
            await request.ToUniTask();

            if (request.asset is not BaseConfigScript config)
            {
                Debug.LogError($"[Config] Could not load '{type}' from Resources/{path}.");
                return null;
            }

            if (config.Type != type)
            {
                Debug.LogError($"[Config] Resource '{path}' contains '{config.Type}', expected '{type}'.");
                return null;
            }

            Debug.Log($"[Config] Loaded '{type}' from Resources/{config.ConfigPath}.");
            return config;
        }
    }
}
