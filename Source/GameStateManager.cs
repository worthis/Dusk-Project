namespace DuskProject.Source
{
    using DuskProject.Source.Enums;

    public class GameStateManager
    {
        private static GameStateManager instance;
        private static object instanceLock = new object();

        private TitleManager _titleManager;
        private ExploreManager _exploreManager;
        private DialogManager _dialogManager;
        private CombatManager _combatManager;

        private GameState _gameState = GameState.Title;
        private bool _redraw = false;
        private bool _quit = false;

        private GameStateManager()
        {
        }

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

        public void ChangeState(GameState gameState)
        {
            _gameState = gameState;

            switch (_gameState)
            {
                case GameState.Info:
                    _exploreManager.UpdateSpells();
                    break;
            }
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
                    _exploreManager.UpdateInfo();
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
                    _exploreManager.RenderInfo();
                    break;
            }
        }

        public void StartCombat(string enemyId, string uniqueFlag = "")
        {
            _combatManager.StartCombat(enemyId, uniqueFlag);
            ChangeState(GameState.Combat);
        }
    }
}
