using System;
using Core;
using Core.Config;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Preloading
{
    public class EntryBootstrap : MonoBehaviour
    {
        private void Start()
        {
            Preloading();
        }

        private async void Preloading()
        {
            try
            {
                await Binding();
                LoadNextScene();
            }
            catch (Exception exception)
            {
                Debug.LogError($"[EntryBootstrap] Preloading failed. The game scene was not loaded.\n{exception}");
            }
        }

        private async UniTask Binding()
        {
            await UniTask.DelayFrame(1);
            IServiceLocator serviceLocator = new Core.ServiceLocator.ServiceLocator();
            serviceLocator.Register<IServiceLocator>(serviceLocator);
            
            ConfigProvider configProvider = new ConfigProvider();
            serviceLocator.Register<IConfigProvider>(configProvider);

            await configProvider.LoadConfigAsync();
        }

        private void LoadNextScene()
        {
            string sceneName = Constants.MenuScene;
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                throw new InvalidOperationException($"Failed to start loading scene '{sceneName}'.");
            }
        }
    }
}
