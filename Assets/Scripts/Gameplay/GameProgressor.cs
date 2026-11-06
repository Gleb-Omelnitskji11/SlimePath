using Core.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gameplay
{
    public class GameProgressor : IService, IInit
    {
        public GameStatus GameStatus { get; private set; }
        public MatchResult MatchResult { get; private set; }
        private int _enemiesCount;

        private UniTask _awaitOperation;

        public void Init()
        {
        }

        public void RepeatGame()
        {
            Clear();
            StartGame();
        }

        public void StartGame()
        {
            SpawnPlayer();
            UpdateInfo();
  
            GameStatus = GameStatus.Running;
            MatchResult = MatchResult.Unfinished;
        }

        private void UpdateInfo()
        {
        }

        public void Pause()
        {
            GameStatus = GameStatus.Pause;
            Time.timeScale = 0;
        }

        public void Resume()
        {
            if (GameStatus != GameStatus.Pause)
                return;
            GameStatus = GameStatus.Running;
            Time.timeScale = 1;
        }

        private void SpawnPlayer()
        {
            
        }
        
        private void Clear()
        {
            
        }
        
    }
}
