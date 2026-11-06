using System;

namespace Core.ServiceLocator
{
    public interface IServiceLocator : IService
    {
        void Register<T>(T service) where T : class, IService;
        void Register(Type serviceType, IService service);
        void RegisterLazy<TInterface, TImplementation>() 
            where TInterface : class, IService 
            where TImplementation : class, TInterface, new();
        void RegisterLazy<TInterface>(Func<TInterface> factory) where TInterface : class, IService;
        T Resolve<T>() where T : class, IService;
        IService Resolve(Type serviceType);
        bool TryResolve<T>(out T service) where T : class, IService;
        void Unregister<T>() where T : class, IService;
        void Clear();
    }
}
