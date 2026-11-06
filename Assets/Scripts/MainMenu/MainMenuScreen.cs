using System;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] private Button _startGameButton;

        private void Start()
        {
            _startGameButton.onClick.AddListener(LoadNextScene);
        }
        
        private void LoadNextScene()
        {
            string sceneName = Constants.GameScene;
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                throw new InvalidOperationException($"Failed to start loading scene '{sceneName}'.");
            }
        }
    }
}