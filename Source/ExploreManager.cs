namespace DuskProject.Source
{
    using DuskProject.Source.Creatures;
    using DuskProject.Source.Dialog;
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;
    using DuskProject.Source.UI;
    using DuskProject.Source.World;

    public class ExploreManager
    {
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
        private string _powerAction = string.Empty;
        private string _powerResult = string.Empty;

        private ImageResource _avatarImage;
        private List<InfoButton> _infoButtons = new List<InfoButton>();

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

            // Miyoo Mini Plus does not have RTC time
            _randGen = new Random(Environment.TickCount);

            _avatarImage = _resourceManager.LoadImage("Data/images/interface/heroine.png");
            ImageResource buttonsImage = _resourceManager.LoadImage("Data/images/interface/action_buttons.png");
            ImageResource buttonSelectedImage = _resourceManager.LoadImage("Data/images/interface/select.png");

            _infoButtons.Add(new InfoButton(ActionType.Attack, 242, 42, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Run, 282, 42, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Heal, 242, 82, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Burn, 282, 82, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Unlock, 242, 122, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Light, 282, 122, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Freeze, 242, 162, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Reflect, 282, 162, 32, 32, buttonsImage));

            foreach (var button in _infoButtons)
            {
                button.SetSelectedImage(buttonSelectedImage, 40, 40, 4, 4);
                button.Selected = button == _infoButtons.First();
                button.Enabled = true;
            }

            Console.WriteLine("ExploreManager initialized");
        }

        public void Start()
        {
            if (_avatar.SaveExists())
            {
                _avatar.Load();
            }
            else
            {
                if (_dialogManager.GetItem("Bare Fists", out var weapon))
                {
                    _avatar.EquipItem(weapon);
                }

                if (_dialogManager.GetItem("Serf Rags", out var armor))
                {
                    _avatar.EquipItem(armor);
                }
            }

            _worldManager.LoadWorld(_avatar.MazeWorld);
            _worldManager.InitScriptedEvents(_avatar.HasCampaignFlag);
        }

        public void Save()
        {
            _avatar.Save();
        }

        public void Update()
        {
            // Update Input
            // Info Screen
            if (_windowManager.KeyPressed(InputKey.KEY_SELECT))
            {
                _message.Clear();
                _soundManager.PlaySound(SoundFX.Click);
                _gameStateManager.ChangeState(GameState.Info);

                return;
            }

            // Main Menu
            if (_windowManager.KeyPressed(InputKey.KEY_START))
            {
                Save();
                _message.Clear();
                _soundManager.StopMusic();
                _soundManager.PlaySound(SoundFX.Click);
                _gameStateManager.ChangeState(GameState.Title);

                return;
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
                    _avatar.MazeWorld = worldPortal.Destination;
                    _worldManager.LoadWorld(worldPortal.Destination);
                    _worldManager.InitScriptedEvents(_avatar.HasCampaignFlag);
                    _message.Start(_worldManager.WorldName);

                    return;
                }

                // Check store entrance
                if (_worldManager.CheckStores(_avatar.X, _avatar.Y, out StorePortal storePortal))
                {
                    _avatar.X = storePortal.DestX;
                    _avatar.Y = storePortal.DestY;
                    _dialogManager.LoadStore(storePortal.Store);
                    _gameStateManager.ChangeState(GameState.Dialog);

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
                    _avatar.Save();
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

        public void UpdateInfo()
        {
            _message.Update();

            // Spell selection
            if (_windowManager.KeyPressed(InputKey.KEY_DOWN) ||
                _windowManager.KeyPressed(InputKey.KEY_RIGHT))
            {
                var buttons = _infoButtons
                    .Where(x => x.Enabled)
                    .ToList();

                var button = buttons.Where(x => x.Selected).First();
                button.Selected = false;

                var index = buttons.IndexOf(button);
                if (index == buttons.Count - 1)
                {
                    buttons.First().Selected = true;
                }
                else
                {
                    buttons[index + 1].Selected = true;
                }
            }

            if (_windowManager.KeyPressed(InputKey.KEY_UP) ||
                _windowManager.KeyPressed(InputKey.KEY_LEFT))
            {
                var buttons = _infoButtons
                    .Where(x => x.Enabled)
                    .ToList();

                var button = buttons.Where(x => x.Selected).First();
                button.Selected = false;

                var index = buttons.IndexOf(button);
                if (index == 0)
                {
                    buttons.Last().Selected = true;
                }
                else
                {
                    buttons[index - 1].Selected = true;
                }
            }

            // Use spell
            if (_windowManager.KeyPressed(InputKey.KEY_A))
            {
                var buttonSelected = _infoButtons.Where(x => x.Selected).First();

                switch (buttonSelected.Action)
                {
                    case ActionType.Heal:
                        DoHealAction();
                        break;

                    case ActionType.Burn:
                        DoBurnAction();
                        break;

                    case ActionType.Unlock:
                        DoUnlockAction();
                        break;
                }
            }

            // Return to Explore
            if (_windowManager.KeyPressed(InputKey.KEY_SELECT) ||
                _windowManager.KeyPressed(InputKey.KEY_B))
            {
                _message.Clear();
                _soundManager.PlaySound(SoundFX.Click);
                _gameStateManager.ChangeState(GameState.Explore);
                return;
            }
        }

        public void UpdateSpells()
        {
            _powerAction = string.Empty;
            _powerResult = string.Empty;

            foreach (var button in _infoButtons)
            {
                button.Selected = false;

                if (button.Action.Equals(ActionType.Attack) ||
                    button.Action.Equals(ActionType.Run))
                {
                    button.Enabled = false;
                    continue;
                }

                if (_avatar.KnowsSpell(Enum.GetName(typeof(ActionType), button.Action)))
                {
                    button.Enabled = true;
                    continue;
                }

                button.Enabled = false;
            }

            _infoButtons.Where(x => x.Enabled).First().Selected = true;
        }

        public void Render()
        {
            _worldManager.RenderBackground(_avatar.Facing);
            _worldManager.RenderWorld(_avatar.X, _avatar.Y, _avatar.Facing);

            // UI
            // Compass
            RenderCompass();

            // todo: Minimap
            // Messages
            _textManager.Render(_message.Text, 160, 200, TextJustify.JUSTIFY_CENTER);
        }

        public void RenderInfo()
        {
            _worldManager.RenderBackground(_avatar.Facing);
            _worldManager.RenderWorld(_avatar.X, _avatar.Y, _avatar.Facing);

            _textManager.Render("INFO", 160, 4, TextJustify.JUSTIFY_CENTER);

            if (_avatar.SpellBookLevel > 0)
            {
                _textManager.Render("SPELLS", 316, 60, TextJustify.JUSTIFY_RIGHT);
            }

            RenderHeroEquipment(ItemType.Armor, 0);                     // Base
            RenderHeroEquipment(ItemType.Armor, _avatar.Armor.Level);   // Armor
            RenderHeroEquipment(ItemType.Weapon, _avatar.Weapon.Level); // Weapon
            RenderHeroStats(true);                                      // HP / MP / Gold
            RenderSpells();

            // Item List
            // Armor
            if (_avatar.Defence > 0)
            {
                _textManager.Render(
                    string.Format(
                        "{0} +{1}",
                        _avatar.Armor.Name,
                        _avatar.Defence),
                    4,
                    130);
            }
            else
            {
                _textManager.Render(_avatar.Armor.Name, 4, 130);
            }

            // Weapon
            if (_avatar.Attack > 0)
            {
                _textManager.Render(
                    string.Format(
                        "{0} +{1}",
                        _avatar.Weapon.Name,
                        _avatar.Attack),
                    4,
                    150);
            }
            else
            {
                _textManager.Render(_avatar.Weapon.Name, 4, 150);
            }

            // Messages
            _textManager.Render(_message.Text, 160, 200, TextJustify.JUSTIFY_CENTER);
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

        private void RenderHeroEquipment(ItemType itemType, int itemLevel)
        {
            _avatarImage.Render(
                itemLevel * 160,
                (int)itemType * 200,
                160,
                200,
                80,
                40);
        }

        private void RenderHeroStats(bool showGold = false)
        {
            _textManager.Render(string.Format("HP {0}/{1}", _avatar.HP, _avatar.MaxHP), 4, 200);
            _textManager.Render(string.Format("MP {0}/{1}", _avatar.MP, _avatar.MaxMP), 4, 220);

            if (showGold)
            {
                _textManager.Render(string.Format("{0} Gold", _avatar.Gold), 316, 220, TextJustify.JUSTIFY_RIGHT);
            }
        }

        private void RenderSpells()
        {
            foreach (var button in _infoButtons)
            {
                button.Render();
            }
        }

        private void DoHealAction()
        {
            if (_avatar.MP <= 0 ||
                _avatar.HP >= _avatar.MaxHP)
            {
                return;
            }

            int healAmount = _randGen.Next((int)(_avatar.MaxHP * 0.5)) + (int)(_avatar.MaxHP * 0.5);
            _avatar.AddHP(healAmount);
            _avatar.AddMP(-1);

            _powerAction = "Heal!";
            _powerResult = string.Format("+{0} HP", healAmount);
            _soundManager.PlaySound(SoundFX.Heal);
        }

        private void DoBurnAction()
        {
            if (_avatar.MP <= 0 ||
                !_avatar.KnowsSpell("Burn"))
            {
                return;
            }

            _avatar.GetFrontTilePos(out int facingTileX, out int facingTileY);

            if (_worldManager.CheckScriptedTiles(facingTileX, facingTileY, out ScriptedTile scriptedTile) &&
                scriptedTile.RequiredAction.Equals("Burn"))
            {
                _avatar.AddMP(-1);
                _avatar.PushCampaignFlag(scriptedTile.UniqueId);
                _worldManager.SetTileId(facingTileX, facingTileY, scriptedTile.AfterTileId);
                _powerAction = "Burn!";
                _powerResult = "Cleared Path!";
                _soundManager.PlaySound(SoundFX.Fire);
            }

            _powerAction = "(No Target)";
            _soundManager.PlaySound(SoundFX.Blocked);
        }

        private void DoUnlockAction()
        {
            if (_avatar.MP <= 0 ||
                !_avatar.KnowsSpell("Unlock"))
            {
                return;
            }

            _avatar.GetFrontTilePos(out int facingTileX, out int facingTileY);

            if (_worldManager.CheckScriptedTiles(facingTileX, facingTileY, out ScriptedTile scriptedTile) &&
                scriptedTile.RequiredAction.Equals("Unlock"))
            {
                _avatar.AddMP(-1);
                _avatar.PushCampaignFlag(scriptedTile.UniqueId);
                _worldManager.SetTileId(facingTileX, facingTileY, scriptedTile.AfterTileId);
                _powerAction = "Unlock!";
                _powerResult = "Door Opened!";
                _soundManager.PlaySound(SoundFX.Unlock);
            }

            _powerAction = "(No Target)";
            _soundManager.PlaySound(SoundFX.Blocked);
        }
    }
}
