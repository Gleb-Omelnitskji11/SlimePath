using Core.ServiceLocator;
using UnityEngine;

namespace Gameplay
{
    public class GameBootstrap : MonoBehaviour
    {
        private IServiceLocator _serviceLocator;

        private void Awake()
        {
            if (!Services.TryGet<IServiceLocator>(out IServiceLocator serviceLocator))
            {
                Debug.LogError("Service Locator could not be found.");
                return;
            }

            _serviceLocator = serviceLocator;
            _serviceLocator.Register<GameProgressor>(new GameProgressor());
            StartGame();
        }

        private void StartGame()
        {
        }

        private void OnDestroy()
        {
            _serviceLocator.Unregister<GameProgressor>();
        }
    }
}
