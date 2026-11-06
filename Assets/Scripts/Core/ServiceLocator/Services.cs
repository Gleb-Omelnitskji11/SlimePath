using UnityEngine;

namespace Core.ServiceLocator
{
    public static class Services
    {
        public static T Get<T>() where T : class, IService
        {
            if (ServiceLocator.Instance != null)
            {
                return ServiceLocator.Instance.Resolve<T>();
            }

            Debug.LogError("ServiceLocator not initialized!");
            return null;

        }

        public static bool TryGet<T>(out T service) where T : class, IService
        {
            if (ServiceLocator.Instance != null)
            {
                return ServiceLocator.Instance.TryResolve(out service);
            }

            service = null;
            return false;
        }
    }
}

