using Core;
using Core.ServiceLocator;
using Gameplay;
using Gameplay.GameServices;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Popups
{
    public class ResultPopup : BasicPopup
    {
        [SerializeField] private TMP_Text _winText;
        [SerializeField] private TMP_Text _loseText;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _exitButton;
        
        private GameProgressor _gameProgressor;

        private void Awake()
        {
            _gameProgressor = Services.Get<GameProgressor>();
            _nextButton.onClick.AddListener(RepeatGame);
            _menuButton.onClick.AddListener(GoToMenu);
            _exitButton.onClick.AddListener(Quit);
        }

        public override void Open()
        {
            base.Open();
            Setup();
        }

        private void Setup()
        {
            bool isWin = _gameProgressor.MatchResult == MatchResult.Win;
            _winText.enabled = isWin;
            _loseText.enabled = !isWin;
        }

        private void RepeatGame()
        {
            Close();
            _gameProgressor.RepeatGame();
        }

        private void GoToMenu()
        {
            Close();
            string sceneName = Constants.MenuScene;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}
