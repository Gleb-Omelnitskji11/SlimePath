using Core;
using Core.ServiceLocator;
using Gameplay;
using Gameplay.GameServices;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Popups
{
    public class PausePopup : BasicPopup
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _quitButton;
        private GameProgressor _gameProgressor;

        private void Awake()
        {
            _resumeButton.onClick.AddListener(Resume);
            _restartButton.onClick.AddListener(Restart);
            _menuButton.onClick.AddListener(GoToMenu);
            _quitButton.onClick.AddListener(Quit);
        }
        
        public override void Open()
        {
            Resolving();
            base.Open();
            _gameProgressor.Pause();
        }

        private void Resolving()
        {
            _gameProgressor = Services.Get<GameProgressor>();
        }

        public override void Close()
        {
            base.Close();
            _anim.onComplete += () => _gameProgressor.Resume();
        }

        private void Resume()
        {
            Close();
        }

        private void Restart()
        {
            Close();
            _gameProgressor.RepeatGame();
        }

        private void GoToMenu()
        {
            Close();
            OnClose += popup =>
            {
                string sceneName = Constants.MenuScene;
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            };
        }


        private void Quit()
        {
            Application.Quit();
        }
    }
}