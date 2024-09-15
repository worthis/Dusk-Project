namespace DuskProject.Source
{
    using DuskProject.Source.Creatures;
    using DuskProject.Source.GameStates.Combat;
    using DuskProject.Source.GameStates.Dialog;
    using DuskProject.Source.GameStates.Explore;
    using DuskProject.Source.GameStates.Menu;
    using DuskProject.Source.GameStates.Title;
    using DuskProject.Source.Interfaces;

    public class GameStateManager
    {
        private static GameStateManager instance;
        private static object instanceLock = new object();

        private WindowManager _windowManager;
        private ResourceManager _resourceManager;
        private SoundManager _soundManager;
        private TextManager _textManager;
        private WorldManager _worldManager;
        private ItemManager _itemManager;
        private ExploreManager _exploreManager;
        private InGameMenuManager _inGameMenuManager;
        private Avatar _avatar;

        private bool _quit = false;

        private GameStateManager()
        {
        }

        public IGameState State { get; set; }

        public (string Id, string UniqueFlag) Enemy { get; set; }

        public string Store { get; set; }

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
            _windowManager = WindowManager.GetInstance();
            _resourceManager = ResourceManager.GetInstance();
            _soundManager = SoundManager.GetInstance();
            _textManager = TextManager.GetInstance();
            _worldManager = WorldManager.GetInstance();
            _itemManager = ItemManager.GetInstance();
            _exploreManager = ExploreManager.GetInstance();
            _inGameMenuManager = InGameMenuManager.GetInstance();
            _avatar = Avatar.GetInstance();

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

        public void Update() => State.Update();

        public void Render() => State.Render();

        public void MainMenu()
        {
            State = new TitleState(
                this,
                _windowManager,
                _resourceManager,
                _soundManager,
                _textManager);
        }

        public void InGameMenu()
        {
            _inGameMenuManager.UpdateSpells();
            State = new MenuState();
        }

        public void StartCombat(string enemyId, string uniqueFlag = "")
        {
            Enemy = (enemyId, uniqueFlag);
            State = new CombatState(
                this,
                _windowManager,
                _resourceManager,
                _soundManager,
                _textManager,
                _worldManager,
                _avatar);
        }

        public void StartDialog(string storeName)
        {
            Store = storeName;
            State = new DialogState(
                this,
                _windowManager,
                _resourceManager,
                _soundManager,
                _textManager,
                _worldManager,
                _itemManager,
                _avatar);
        }

        public void StartExplore()
        {
            State = new ExploreState();
        }

        public void StartGame()
        {
            _exploreManager.Start();
            StartExplore();
        }
    }
}
