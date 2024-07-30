namespace DuskProject.Source
{
    public enum GameState : byte
    {
        Title = 0,
        Explore = 1,
        Combat = 2,
        Dialog = 3,
        Info = 4,
    }

    public class GameStateManager
    {
        private static GameStateManager instance;
        private static object instanceLock = new object();

        private GameState _gameState = GameState.Title;
        private bool _redraw = false;
        private bool _quit = false;

        private TitleManager _titleManager;
        private ExploreManager _exploreManager;

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

                    break;

                case GameState.Dialog:
                    break;

                case GameState.Info:

                    break;

                default:

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

                    break;

                case GameState.Dialog:

                    break;

                case GameState.Info:

                    break;

                default:

                    break;
            }
        }
    }
}
