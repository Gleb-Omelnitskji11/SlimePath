using System;
using Core.ServiceLocator;

namespace Core.ObjectPool
{
    public interface IObjectPooler : IService
    {
        void CreatePool<T>(string key, Func<T> factory, Action<T> onGet = null, Action<T> onRelease = null, uint prewarmCount = 0) where T : class;
        T Get<T>(string key) where T : class;
        void Release<T>(string key, T instance) where T : class;
        void Clear(string key);
        void ClearAll();
    }
}
