using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Config
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly Dictionary<ConfigType, BaseConfigScript> _configs = new Dictionary<ConfigType, BaseConfigScript>();
        
        public async UniTask LoadConfigAsync()
        {
            _configs.Clear();
            List<ConfigType> missingConfigs = new List<ConfigType>();
            foreach (ConfigType type in Enum.GetValues(typeof(ConfigType)))
            {
                BaseConfigScript config = await BaseConfigScript.LoadAsync(type);
                if (config != null)
                    _configs[type] = config;
                else
                    missingConfigs.Add(type);
            }

            if (missingConfigs.Count > 0)
                throw new InvalidOperationException($"Required configs failed to load: {string.Join(", ", missingConfigs)}.");
        }

        public T GetConfig<T>(ConfigType type) where T : BaseConfigScript
        {
            if (_configs.TryGetValue(type, out BaseConfigScript config) && config is T typedConfig)
                return typedConfig;

            Debug.LogError($"[ConfigProvider] Config '{type}' of type '{typeof(T).Name}' is not loaded.");
            return null;
        }
    }
}
