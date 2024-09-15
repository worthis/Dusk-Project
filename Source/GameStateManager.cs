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
        private const string SaveFilePath = "Save/avatar.json";

        private static readonly object InstanceLock = new object();
        private static GameStateManager instance;

        private WindowManager _windowManager;
        private ResourceManager _resourceManager;
        private SoundManager _soundManager;
        private TextManager _textManager;
        private WorldManager _worldManager;
        private ItemManager _itemManager;
        private Avatar _avatar;

        private bool _isQuitting = false;
        private bool _isGameStarted = false;
        private IGameState _currentState;

        private GameStateManager()
        {
            Console.WriteLine("GameStateManager created");
        }

        public static GameStateManager Instance
        {
            get
            {
                lock (InstanceLock)
                {
                    return instance ??= new GameStateManager();
                }
            }
        }

        public (string Id, string UniqueFlag) Enemy { get; private set; }

        public string Store { get; private set; }

        public bool IsQuitting => _isQuitting;

        public static bool SaveFileExists() => File.Exists(SaveFilePath);

        public void Init()
        {
            _windowManager = WindowManager.Instance;
            _resourceManager = ResourceManager.Instance;
            _soundManager = SoundManager.Instance;
            _textManager = TextManager.Instance;
            _worldManager = WorldManager.Instance;
            _itemManager = ItemManager.Instance;
            _avatar = Avatar.Instance;

            Console.WriteLine("GameStateManager initialized");
        }

        public void RegisterQuit() => _isQuitting = true;

        public void Update() => _currentState?.Update();

        public void Render() => _currentState?.Render();

        public void ShowMainMenu()
        {
            _isGameStarted = false;
            _currentState = new TitleState(
                this,
                _windowManager,
                _resourceManager,
                _soundManager,
                _textManager);
        }

        public void ShowInGameMenu()
        {
            _currentState = new MenuState(
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
            _currentState = new CombatState(
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
            _currentState = new DialogState(
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
            _currentState = new ExploreState(
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
                LoadGame();
                StartExplore();
                return;
            }

            InitializeNewGame();
            StartDialog("8-a-nightmare");
            _ = SaveGame();
        }

        public async Task SaveGame()
        {
            if (!_isGameStarted)
            {
                return;
            }

            string serializedAvatar = _avatar.Serialize();
            await File.WriteAllTextAsync(SaveFilePath, serializedAvatar);

            Console.WriteLine("Game saved.");
        }

        public void LoadGame()
        {
            string jsonData = File.ReadAllText(SaveFilePath);
            _avatar.Deserialize(jsonData);
            _isGameStarted = true;
            _worldManager.LoadWorld(_avatar.World);
            _worldManager.InitScriptedEvents(_avatar.HasCampaignFlag);

            Console.WriteLine("Game loaded.");
        }

        private void InitializeNewGame()
        {
            _avatar.Reset();
            EquipStartingItems();
            _isGameStarted = true;
            _worldManager.LoadWorld(_avatar.World);
            _worldManager.InitScriptedEvents(_avatar.HasCampaignFlag);
        }

        private void EquipStartingItems()
        {
            if (_itemManager.GetItem("Bare Fists", out var weapon))
            {
                _avatar.EquipItem(weapon);
            }

            if (_itemManager.GetItem("Serf Rags", out var armor))
            {
                _avatar.EquipItem(armor);
            }
        }
    }
}
