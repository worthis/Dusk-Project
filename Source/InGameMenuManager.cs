namespace DuskProject.Source
{
    using DuskProject.Source.Creatures;
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;
    using DuskProject.Source.UI;
    using DuskProject.Source.World;

    public class InGameMenuManager
    {
        private static InGameMenuManager instance;
        private static object instanceLock = new object();

        private WindowManager _windowManager;
        private ResourceManager _resourceManager;
        private GameStateManager _gameStateManager;
        private SoundManager _soundManager;
        private TextManager _textManager;
        private WorldManager _worldManager;
        private Avatar _avatar;

        private ImageResource _avatarImage;
        private List<InfoButton> _infoButtons = new List<InfoButton>();

        private string _powerAction = string.Empty;
        private string _powerResult = string.Empty;

        private Random _randGen;

        private InGameMenuManager()
        {
        }

        public static InGameMenuManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new InGameMenuManager();
                        Console.WriteLine("InGameMenuManager created");
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
            _avatar = Avatar.GetInstance();

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

            // Miyoo Mini Plus does not have RTC time
            _randGen = new Random(Environment.TickCount);

            Console.WriteLine("InGameMenuManager initialized");
        }

        public void Update()
        {
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
                _soundManager.PlaySound(SoundFX.Click);
                _gameStateManager.StartExplore();
                return;
            }
        }

        public void Render()
        {
            _textManager.Render("INFO", 160, 4, TextJustify.JUSTIFY_CENTER);

            if (_avatar.SpellBookLevel > 0)
            {
                _textManager.Render("SPELLS", 316, 60, TextJustify.JUSTIFY_RIGHT);
            }

            RenderHeroEquipment(ItemType.Armor, 0);                             // Base
            RenderHeroEquipment(ItemType.Armor, _avatar.Armor?.Level ?? 0);     // Armor
            RenderHeroEquipment(ItemType.Weapon, _avatar.Weapon?.Level ?? 0);   // Weapon
            RenderHeroStats(true);                                              // HP / MP / Gold
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
                _textManager.Render(_avatar.Armor?.Name, 4, 130);
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
                _textManager.Render(_avatar.Weapon?.Name, 4, 150);
            }

            // Messages
            _textManager.Render(_powerAction, 4, 60);
            _textManager.Render(_powerResult, 4, 80);
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

            var buttonFirst = _infoButtons.Where(x => x.Enabled).FirstOrDefault();
            if (buttonFirst is not null)
            {
                buttonFirst.Selected = true;
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
