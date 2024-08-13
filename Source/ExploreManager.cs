namespace DuskProject.Source
{
    using DuskProject.Source.Creatures;
    using DuskProject.Source.Dialog;
    using DuskProject.Source.Enums;
    using DuskProject.Source.UI;
    using DuskProject.Source.World;

    public class ExploreManager
    {
        private const string _saveFile = "Save/avatar.json";

        private const int _encounterIncrement = 5;
        private const int _encounterChanceMax = 30;

        private static ExploreManager instance;
        private static object instanceLock = new object();

        private WindowManager _windowManager;
        private ResourceManager _resourceManager;
        private GameStateManager _gameStateManager;
        private SoundManager _soundManager;
        private TextManager _textManager;
        private WorldManager _worldManager;
        private DialogManager _dialogManager;
        private Avatar _avatar;

        private TimedMessage _message = new TimedMessage(timeOut: 2000);

        private Random _randGen;
        private int _encounterChance = 0;
        private bool _gameStarted = false;

        private Minimap _minimap;

        private ExploreManager()
        {
        }

        public static ExploreManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ExploreManager();
                        Console.WriteLine("ExploreManager created");
                    }
                }
            }

            return instance;
        }

        public void Init()
        {
            _windowManager = WindowManager.GetInstance();
            _resourceManager = ResourceManager.GetInstance();
            _gameStateManager = GameStateManager.GetInstance();
            _soundManager = SoundManager.GetInstance();
            _textManager = TextManager.GetInstance();
            _worldManager = WorldManager.GetInstance();
            _dialogManager = DialogManager.GetInstance();
            _avatar = Avatar.GetInstance();

            Directory.CreateDirectory(Path.GetDirectoryName(_saveFile));

            // Miyoo Mini Plus does not have RTC time
            _randGen = new Random(Environment.TickCount);

            Console.WriteLine("ExploreManager initialized");
        }

        public void Start()
        {
            if (SaveFileExists())
            {
                Load();
                LoadWorld(_avatar.World);
                _gameStarted = true;

                return;
            }

            _avatar.Reset();

            if (_dialogManager.GetItem("Bare Fists", out var weapon))
            {
                _avatar.EquipItem(weapon);
            }

            if (_dialogManager.GetItem("Serf Rags", out var armor))
            {
                _avatar.EquipItem(armor);
            }

            _ = Save();

            LoadWorld(_avatar.World);
            _gameStateManager.StartDialog("8-a-nightmare");
            _gameStarted = true;
        }

        public void LoadWorld(string worldName)
        {
            _avatar.World = worldName;
            _worldManager.LoadWorld(_avatar.World);
            _worldManager.InitScriptedEvents(_avatar.HasCampaignFlag);

            bool showMinimap = _minimap?.Enabled ?? false;

            // Init minimap
            _minimap = new Minimap(
                _worldManager.Width,
                _worldManager.Height,
                _resourceManager.LoadImage("Data/images/interface/minimap.png"),
                _resourceManager.LoadImage("Data/images/interface/minimap_cursor.png"));

            _minimap.X = 4;
            _minimap.Y = 4;
            _minimap.Enabled = showMinimap;

            for (int i = 0; i < _worldManager.Width; i++)
            {
                for (int j = 0; j < _worldManager.Height; j++)
                {
                    // Portals
                    if (_worldManager.CheckPortals(i, j, out WorldPortal worldPortal))
                    {
                        _minimap.SetTileType(i, j, MinimapTileType.Portal);
                        continue;
                    }

                    // Stores
                    if (_worldManager.CheckStores(i, j, out StorePortal storePortal))
                    {
                        _minimap.SetTileType(i, j, MinimapTileType.Store);
                        continue;
                    }

                    Tile tile = _worldManager.GetTile(i, j);
                    _minimap.SetTileType(i, j, tile.Walkable ? MinimapTileType.Walkable : MinimapTileType.Blocked);
                }
            }
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
        }

        public bool SaveFileExists()
        {
            return File.Exists(_saveFile);
        }

        public void Load()
        {
            if (!SaveFileExists())
            {
                return;
            }

            using (StreamReader streamReader = new(_saveFile))
            {
                string jsonData = streamReader.ReadToEnd();
                _avatar.Deserialize(jsonData);
            }
        }

        public void SetWorldRenderOffset(int offsetX, int offsetY)
        {
            _worldManager.TileSetRenderOffsetX = offsetX;
            _worldManager.TileSetRenderOffsetY = offsetY;
        }

        public void Update()
        {
            // Update Input
            // Info Screen
            if (_windowManager.KeyPressed(InputKey.KEY_SELECT))
            {
                _soundManager.PlaySound(SoundFX.Click);
                _gameStateManager.InGameMenu();

                return;
            }

            // Main Menu
            if (_windowManager.KeyPressed(InputKey.KEY_START))
            {
                _ = Save();
                _soundManager.StopMusic();
                _soundManager.PlaySound(SoundFX.Click);
                _gameStateManager.MainMenu();
                _gameStarted = false;

                return;
            }

            // Minimap
            if (_windowManager.KeyPressed(InputKey.KEY_X))
            {
                _minimap.Enabled = !_minimap.Enabled;
            }

            // Avatar Movement
            _avatar.Moved = false;

            if (_windowManager.KeyPressed(InputKey.KEY_UP))
            {
                _avatar.GetFrontTilePos(out int posX, out int posY);
                Tile tile = _worldManager.GetTile(posX, posY);

                if (tile is not null &&
                    tile.Walkable)
                {
                    _avatar.X = posX;
                    _avatar.Y = posY;
                    _avatar.Moved = true;
                }
                else
                {
                    _soundManager.PlaySound(SoundFX.Blocked);
                }
            }

            if (_windowManager.KeyPressed(InputKey.KEY_DOWN))
            {
                _avatar.GetBehindTilePos(out int posX, out int posY);
                Tile tile = _worldManager.GetTile(posX, posY);

                if (tile is not null &&
                    tile.Walkable)
                {
                    _avatar.X = posX;
                    _avatar.Y = posY;
                    _avatar.Moved = true;
                }
                else
                {
                    _soundManager.PlaySound(SoundFX.Blocked);
                }
            }

            if (_windowManager.KeyPressed(InputKey.KEY_LEFT))
            {
                _avatar.TurnLeft();
            }

            if (_windowManager.KeyPressed(InputKey.KEY_RIGHT))
            {
                _avatar.TurnRight();
            }

            _message.Update();
            _textManager.Color = _avatar.IsBadlyHurt() ? TextColor.Red : TextColor.Default;

            if (_avatar.Moved)
            {
                // Check exit portals
                if (_worldManager.CheckPortals(_avatar.X, _avatar.Y, out WorldPortal worldPortal))
                {
                    _avatar.X = worldPortal.DestX;
                    _avatar.Y = worldPortal.DestY;
                    LoadWorld(worldPortal.Destination);
                    _message.Start(_worldManager.WorldName);

                    return;
                }

                // Check store entrance
                if (_worldManager.CheckStores(_avatar.X, _avatar.Y, out StorePortal storePortal))
                {
                    _avatar.X = storePortal.DestX;
                    _avatar.Y = storePortal.DestY;
                    _gameStateManager.StartDialog(storePortal.Store);

                    return;
                }

                // Special scripts
                // Message Point
                if (_worldManager.CheckMessagePoints(_avatar.X, _avatar.Y, out MessagePoint messagePoint))
                {
                    if (!_avatar.HasCampaignFlag(messagePoint.UniqueId))
                    {
                        _message.Start(messagePoint.Message);
                        _soundManager.PlaySound(messagePoint.Sound);
                        _avatar.PushCampaignFlag(messagePoint.UniqueId);
                    }
                }

                // Rest Points
                if (_worldManager.CheckRestPoints(_avatar.X, _avatar.Y, out RestPoint restPoint))
                {
                    _avatar.Sleep();
                    _ = Save();
                    _message.Start(restPoint.Message);
                    _soundManager.PlaySound(restPoint.Sound);

                    return;
                }

                // Chests
                if (_worldManager.CheckChests(_avatar.X, _avatar.Y, out ChestPoint chestPoint))
                {
                    // todo: render treasure icon
                    if (!_avatar.HasCampaignFlag(chestPoint.UniqueId))
                    {
                        switch (chestPoint.RewardType)
                        {
                            case ChestRewardType.Gold:
                                _avatar.AddGold(chestPoint.RewardItemAmount);
                                break;

                            case ChestRewardType.Weapon:
                            case ChestRewardType.Armor:
                                if (_dialogManager.GetItem(chestPoint.RewardItemId, out Item item))
                                {
                                    if (_avatar.IsBetterItem(item))
                                    {
                                        _avatar.EquipItem(item);
                                    }
                                }

                                break;

                            case ChestRewardType.Spell:
                                if (_dialogManager.GetItem(chestPoint.RewardItemId, out Item spell))
                                {
                                    _avatar.LearnSpell(spell);
                                }

                                break;

                            case ChestRewardType.PowerUp:
                                if (chestPoint.RewardItemId.Equals("Magic Sapphire (MP Up)"))
                                {
                                    _avatar.MaxMP += 2;
                                    _avatar.AddMP(2);
                                }

                                if (chestPoint.RewardItemId.Equals("Magic Emerald (HP Up)"))
                                {
                                    _avatar.MaxHP += 5;
                                    _avatar.AddHP(5);
                                }

                                if (chestPoint.RewardItemId.Equals("Magic Ruby (Atk Up)"))
                                {
                                    _avatar.Attack++;
                                }

                                if (chestPoint.RewardItemId.Equals("Magic Diamond (Def Up)"))
                                {
                                    _avatar.Defence++;
                                }

                                break;
                        }

                        _worldManager.SetTileId(chestPoint.X, chestPoint.Y, chestPoint.OpenedTileId);
                        _avatar.PushCampaignFlag(chestPoint.UniqueId);
                        _soundManager.PlaySound(chestPoint.Sound);

                        if (chestPoint.RewardItemAmount > 1)
                        {
                            _message.Start(string.Format("Found {0} {1}!", chestPoint.RewardItemAmount, chestPoint.RewardItemId));
                        }
                        else
                        {
                            _message.Start(string.Format("Found {0}!", chestPoint.RewardItemId));
                        }

                        return;
                    }
                }

                // Scripted Enemies
                // todo: world update on boss kill
                if (_worldManager.CheckScriptedEnemies(_avatar.X, _avatar.Y, out ScriptedEnemy scriptedEnemy))
                {
                    if (!_avatar.HasCampaignFlag(scriptedEnemy.UniqueId))
                    {
                        _encounterChance = 0;
                        _message.Clear();
                        _gameStateManager.StartCombat(scriptedEnemy.EnemyId, scriptedEnemy.UniqueId);

                        return;
                    }
                }

                // Encounters
                if (_worldManager.Enemies is not null &&
                    _worldManager.Enemies.Count > 0)
                {
                    if (_randGen.Next(100) < _encounterChance)
                    {
                        _encounterChance = 0;
                        _message.Clear();
                        var enemyIndex = _randGen.Next(_worldManager.Enemies.Count);
                        _gameStateManager.StartCombat(_worldManager.Enemies[enemyIndex]);

                        return;
                    }

                    _encounterChance += _encounterIncrement;
                    if (_encounterChance > _encounterChanceMax)
                    {
                        _encounterChance = _encounterChanceMax;
                    }
                }
            }
        }

        public void Render()
        {
            _worldManager.RenderBackground(_avatar.Facing);
            _worldManager.RenderWorld(_avatar.X, _avatar.Y, _avatar.Facing);

            // UI
            // Compass
            RenderCompass();

            // Minimap
            _minimap.Render();
            _minimap.RenderHero(_avatar.X, _avatar.Y, _avatar.Facing);

            // Messages
            _textManager.Render(_message.Text, 160, 200, TextJustify.JUSTIFY_CENTER);
        }

        public void RenderWorld()
        {
            _worldManager.RenderBackground(_avatar.Facing);
            _worldManager.RenderWorld(_avatar.X, _avatar.Y, _avatar.Facing);
        }

        private void RenderCompass()
        {
            switch (_avatar.Facing)
            {
                case AvatarFacing.North:
                    _textManager.Render("NORTH", 160, 4, TextJustify.JUSTIFY_CENTER);
                    break;

                case AvatarFacing.East:
                    _textManager.Render("EAST", 160, 4, TextJustify.JUSTIFY_CENTER);
                    break;

                case AvatarFacing.West:
                    _textManager.Render("WEST", 160, 4, TextJustify.JUSTIFY_CENTER);
                    break;

                case AvatarFacing.South:
                    _textManager.Render("SOUTH", 160, 4, TextJustify.JUSTIFY_CENTER);
                    break;
            }
        }
    }
}
