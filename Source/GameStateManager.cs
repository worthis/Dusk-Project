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
        private const string _saveFile = "Save/avatar.json";

        private static GameStateManager instance;
        private static object instanceLock = new object();

        private WindowManager _windowManager;
        private ResourceManager _resourceManager;
        private SoundManager _soundManager;
        private TextManager _textManager;
        private WorldManager _worldManager;
        private ItemManager _itemManager;
        private Avatar _avatar;

        private bool _quit = false;
        private bool _gameStarted = false;
        private IGameState _state;

        private GameStateManager()
        {
        }

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

        public void Update() => _state.Update();

        public void Render() => _state.Render();

        public void MainMenu()
        {
            _gameStarted = false;
            _state = new TitleState(
                this,
                _windowManager,
                _resourceManager,
                _soundManager,
                _textManager);
        }

        public void InGameMenu()
        {
            _state = new MenuState(
                this,
                _windowManager,
                _resourceManager,
                _soundManager,
                _textManager,
                _worldManager,
                _avatar);
        }

        public void StartCombat(string enemyId, string uniqueFlag = "")
        {
            Enemy = (enemyId, uniqueFlag);
            _state = new CombatState(
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
            _state = new DialogState(
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
            _state = new ExploreState(
                this,
                _windowManager,
                _resourceManager,
                _soundManager,
                _textManager,
                _worldManager,
                _itemManager,
                _avatar);
        }

        public void StartGame()
        {
            if (SaveFileExists())
            {
                using (StreamReader streamReader = new(_saveFile))
                {
                    string jsonData = streamReader.ReadToEnd();
                    _avatar.Deserialize(jsonData);
                }

                _gameStarted = true;
                _worldManager.LoadWorld(_avatar.World);
                _worldManager.InitScriptedEvents(_avatar.HasCampaignFlag);
                StartExplore();

                return;
            }

            _avatar.Reset();

            if (_itemManager.GetItem("Bare Fists", out var weapon))
            {
                _avatar.EquipItem(weapon);
            }

            if (_itemManager.GetItem("Serf Rags", out var armor))
            {
                _avatar.EquipItem(armor);
            }

            _gameStarted = true;
            _worldManager.LoadWorld(_avatar.World);
            _worldManager.InitScriptedEvents(_avatar.HasCampaignFlag);
            StartDialog("8-a-nightmare");
            _ = Save();
        }

        public bool SaveFileExists()
        {
            return File.Exists(_saveFile);
        }

        public async Task Save()
        {
            if (!_gameStarted)
            {
                return;
            }

            string avatarSerialized = _avatar.Serialize();

            using (StreamWriter streamWriter = new StreamWriter(_saveFile))
            {
                await streamWriter.WriteAsync(avatarSerialized);
                await streamWriter.FlushAsync();
            }

            Console.WriteLine("Game saved.");
        }
    }
}
