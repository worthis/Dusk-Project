namespace DuskProject.Source
{
    using DuskProject.Source.Enums;
    using DuskProject.Source.Maze;
    using DuskProject.Source.Resources;
    using DuskProject.Source.UI;

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
        private MazeWorldManager _mazeWorldManager;
        private DialogManager _dialogManager;
        private Avatar _avatar;

        private TimedMessage _message = new TimedMessage(timeOut: 2000);

        private Random _randGen;
        private int _encounterChance = 0;

        private ImageResource _avatarImage;
        private List<InfoButton> _infoButtons = new List<InfoButton>();
        private int _infoButtonsCursor = 0;

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
            _mazeWorldManager = MazeWorldManager.GetInstance();
            _dialogManager = DialogManager.GetInstance();
            _avatar = Avatar.GetInstance();

            // Miyoo Mini Plus does not have RTC time
            _randGen = new Random(Environment.TickCount);

            _avatarImage = _resourceManager.LoadImage("Data/images/interface/heroine.png");
            ImageResource buttonsImage = _resourceManager.LoadImage("Data/images/interface/action_buttons.png");
            ImageResource buttonSelectedImage = _resourceManager.LoadImage("Data/images/interface/select.png");

            /*_infoButtons = new InfoButton[8]
            {
                new InfoButton(ActionType.Attack, 238, 38, 32, 32, buttonsImage),
                new InfoButton(ActionType.Heal, 238, 78, 32, 32, buttonsImage),
                new InfoButton(ActionType.Unlock, 238, 118, 32, 32, buttonsImage),
                new InfoButton(ActionType.Freeze, 238, 158, 32, 32, buttonsImage),
                new InfoButton(ActionType.Run, 278, 38, 32, 32, buttonsImage),
                new InfoButton(ActionType.Burn, 278, 78, 32, 32, buttonsImage),
                new InfoButton(ActionType.Light, 278, 118, 32, 32, buttonsImage),
                new InfoButton(ActionType.Reflect, 278, 158, 32, 32, buttonsImage),
            };*/

            _infoButtons.Add(new InfoButton(ActionType.Attack, 238, 38, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Heal, 238, 78, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Unlock, 238, 118, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Freeze, 238, 158, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Run, 278, 38, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Burn, 278, 78, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Light, 278, 118, 32, 32, buttonsImage));
            _infoButtons.Add(new InfoButton(ActionType.Reflect, 278, 158, 32, 32, buttonsImage));

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

            _mazeWorldManager.LoadMazeWorld(_avatar.MazeWorld);

            // Test
            /*_mazeWorldManager.LoadMazeWorld("5-cedar-village");
            _avatar.PosX = 6;
            _avatar.PosY = 5;
            _avatar.AddGold(1000);*/
        }

        public void Save()
        {
            _avatar.Save();
        }

        public void Update()
        {
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
                _soundManager.PlaySound(SoundFX.Click);
                _gameStateManager.ChangeState(GameState.Title);
                return;
            }

            _avatar.Update();
            _message.Update();
            _textManager.Color = _avatar.IsBadlyHurt() ? TextColor.Red : TextColor.Default;

            if (_avatar.Moved)
            {
                // Check exit portals
                if (_mazeWorldManager.CheckPortals(_avatar.PosX, _avatar.PosY, out MazePortal mazePortal))
                {
                    _avatar.PosX = mazePortal.DestX;
                    _avatar.PosY = mazePortal.DestY;
                    _avatar.MazeWorld = mazePortal.Destination;

                    _mazeWorldManager.LoadMazeWorld(mazePortal.Destination);

                    _message.Start(_mazeWorldManager.MazeWorldName);

                    return;
                }

                // Check store entrance
                if (_mazeWorldManager.CheckStores(_avatar.PosX, _avatar.PosY, out StorePortal storePortal))
                {
                    _avatar.PosX = storePortal.DestX;
                    _avatar.PosY = storePortal.DestY;

                    _dialogManager.LoadStore(storePortal.Store);

                    _gameStateManager.ChangeState(GameState.Dialog);

                    return;
                }

                // Encounters
                if (_mazeWorldManager.Enemies is not null &&
                    _mazeWorldManager.Enemies.Count > 0)
                {
                    if (_randGen.Next(100) < _encounterChance)
                    {
                        _encounterChance = 0;
                        _message.Clear();
                        var enemyIndex = _randGen.Next(_mazeWorldManager.Enemies.Count);
                        _gameStateManager.StartCombat(_mazeWorldManager.Enemies[enemyIndex]);

                        return;
                    }

                    _encounterChance += _encounterIncrement;
                    if (_encounterChance > _encounterChanceMax)
                    {
                        _encounterChance = _encounterChanceMax;
                    }
                }

                // Special scripts
            }
        }

        public void UpdateInfo()
        {
            _message.Update();

            // Spell selection
            if (_windowManager.KeyPressed(InputKey.KEY_UP) ||
                _windowManager.KeyPressed(InputKey.KEY_LEFT))
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

            if (_windowManager.KeyPressed(InputKey.KEY_DOWN) ||
                _windowManager.KeyPressed(InputKey.KEY_RIGHT))
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
            foreach (var button in _infoButtons)
            {
                if (button.Action.Equals(ActionType.Attack) ||
                    button.Action.Equals(ActionType.Run))
                {
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
            // Maze Cell
            _mazeWorldManager.RenderBackground(_avatar.Facing);
            _mazeWorldManager.Render(_avatar.PosX, _avatar.PosY, _avatar.Facing);

            // UI
            // Compass
            RenderCompass();

            // Minimap
            // Messages
            _textManager.Render(_message.Text, 160, 200, TextJustify.JUSTIFY_CENTER);
        }

        public void RenderInfo()
        {
            // Maze Cell
            _mazeWorldManager.RenderBackground(_avatar.Facing);
            _mazeWorldManager.Render(_avatar.PosX, _avatar.PosY, _avatar.Facing);

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
    }
}
