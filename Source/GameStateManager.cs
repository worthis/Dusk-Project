namespace DuskProject.Source
{
    using DuskProject.Source.Enums;

    public class GameStateManager
    {
        private static GameStateManager instance;
        private static object instanceLock = new object();

        private TitleManager _titleManager;
        private ExploreManager _exploreManager;
        private InGameMenuManager _inGameMenuManager;
        private DialogManager _dialogManager;
        private CombatManager _combatManager;

        private GameState _gameState = GameState.Title;
        private bool _quit = false;

        private GameStateManager()
        {
        }

        public GameState State { get => _gameState; }

        public static GameStateManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new GameStateManager();
                        Console.WriteLine("GameStateManager created");
                    }
                }
            }

            return instance;
        }

        public void Init()
        {
            _titleManager = TitleManager.GetInstance();
            _exploreManager = ExploreManager.GetInstance();
            _inGameMenuManager = InGameMenuManager.GetInstance();
            _dialogManager = DialogManager.GetInstance();
            _combatManager = CombatManager.GetInstance();

            Console.WriteLine("GameStateManager initialized");
        }

        public bool Quitting()
        {
            return _quit;
        }

        public void RegisterQuit()
        {
            _quit = true;
        }

        public void Update()
        {
            switch (_gameState)
            {
                case GameState.Title:
                    _titleManager.Update();
                    break;

                case GameState.Explore:
                    _exploreManager.Update();
                    break;

                case GameState.Combat:
                    _combatManager.Update();
                    break;

                case GameState.Dialog:
                    _dialogManager.Update();
                    break;

                case GameState.Info:
                    _inGameMenuManager.Update();
                    break;
            }
        }

        public void Render()
        {
            switch (_gameState)
            {
                case GameState.Title:
                    _titleManager.Render();
                    break;

                case GameState.Explore:
                    _exploreManager.Render();
                    break;

                case GameState.Combat:
                    _combatManager.Render();
                    break;

                case GameState.Dialog:
                    _dialogManager.Render();
                    break;

                case GameState.Info:
                    _exploreManager.RenderWorld();
                    _inGameMenuManager.Render();
                    break;
            }
        }

        public void MainMenu()
        {
            _gameState = GameState.Title;
        }

        public void InGameMenu()
        {
            _inGameMenuManager.UpdateSpells();
            _gameState = GameState.Info;
        }

        public void StartCombat(string enemyId, string uniqueFlag = "")
        {
            _combatManager.StartCombat(enemyId, uniqueFlag);
            _gameState = GameState.Combat;
        }

        public void StartDialog(string storeName)
        {
            _dialogManager.StartDialog(storeName);
            _gameState = GameState.Dialog;
        }

        public void StartExplore()
        {
            _gameState = GameState.Explore;
        }

        public void StartGame()
        {
            _exploreManager.Start();
            StartExplore();
        }
    }
}
