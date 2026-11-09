using Core.ServiceLocator;
using UnityEngine;

namespace Gameplay.GameServices
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private PlayerHolder _playerHolder;
        [SerializeField] private LevelGenerator _levelGenerator;
        private IServiceLocator _serviceLocator;

        private void Awake()
        {
            if (!Services.TryGet<IServiceLocator>(out IServiceLocator serviceLocator))
            {
                Debug.LogError("Service Locator could not be found.");
                return;
            }

            _serviceLocator = serviceLocator;
            var progressor = new GameProgressor();
            _serviceLocator.Register<LevelGenerator>(_levelGenerator);
            _serviceLocator.Register<PlayerHolder>(_playerHolder);
            _serviceLocator.Register<GameProgressor>(progressor);
        }

        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            var progressor = Services.Get<GameProgressor>();
            progressor.StartGame();
        }

        private void OnDestroy()
        {
            _serviceLocator.Unregister<GameProgressor>();
            _serviceLocator.Unregister<PlayerHolder>();
            _serviceLocator.Unregister<LevelGenerator>();
        }
    }
}
