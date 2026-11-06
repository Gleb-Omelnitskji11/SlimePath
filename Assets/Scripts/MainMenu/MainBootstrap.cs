using Core.ObjectPool;
using Core.PopupShower;
using Core.ServiceLocator;
using UnityEngine;

namespace MainMenu
{
    public class MainBootstrap : MonoBehaviour
    {
        [SerializeField] private PopupsProvider _popupsProvider;
        
        private void Awake()
        {
            if (!Services.TryGet<IServiceLocator>(out IServiceLocator serviceLocator))
            {
                Debug.LogError("Service Locator could not be found.");
                return;
            }
            
            Binding(serviceLocator);
        }
        
        private void Binding(IServiceLocator serviceLocator)
        {
            bool isInited = Services.TryGet<IObjectPooler>(out IObjectPooler objectPooler);
            
            if (isInited)
            {
                return;
            }
            
            Initialising(serviceLocator);
        }

        private void Initialising(IServiceLocator serviceLocator)
        {
            serviceLocator.Register<IObjectPooler>(new ObjectPooler());
            PopupsProvider popupsProvider = Instantiate(_popupsProvider);
            serviceLocator.Register<IPopupsProvider>(popupsProvider);
            DontDestroyOnLoad(popupsProvider);
        }
    }
}