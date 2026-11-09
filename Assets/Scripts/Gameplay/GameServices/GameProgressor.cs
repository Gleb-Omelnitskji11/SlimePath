using Core.PopupShower;
using Core.ServiceLocator;
using Cysharp.Threading.Tasks;

namespace Gameplay.GameServices
{
    public class GameProgressor : IService, IInit
    {
        public GameStatus GameStatus { get; private set; }
        public MatchResult MatchResult { get; private set; }
        private int _enemiesCount;

        private UniTask _awaitOperation;
        private IPopupsProvider _popupsProvider;
        private PlayerHolder _playerHolder;
        private LevelGenerator _levelGenerator;

        public void Init()
        {
            _popupsProvider = Services.Get<IPopupsProvider>();
            _playerHolder = Services.Get<PlayerHolder>();
            _levelGenerator = Services.Get<LevelGenerator>();
        }

        public void RepeatGame()
        {
            Clear();
            StartGame();
        }

        public void StartGame()
        {
            InitPlayer();

            GameStatus = GameStatus.Running;
            MatchResult = MatchResult.Unfinished;
        }

        public void Pause()
        {
            GameStatus = GameStatus.Pause;
            //Time.timeScale = 0;
        }

        public void Resume()
        {
            if (GameStatus != GameStatus.Pause)
                return;
            GameStatus = GameStatus.Running;
            //Time.timeScale = 1;
        }

        public void GameFinished(bool win)
        {
            GameStatus = GameStatus.NotStarted;
            MatchResult = win ? MatchResult.Win : MatchResult.Lose;
            _popupsProvider.TryOpen(PopupID.ResultPopup, true);
        }

        private void InitPlayer()
        {
            _playerHolder.Player.StartGame();
            _levelGenerator.Build();
            
            _playerHolder.Player.ApplyVisual();
        }

        private void Clear()
        {
        }
    }
}