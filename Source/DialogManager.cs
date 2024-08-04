namespace DuskProject.Source
{
    using System;
    using DuskProject.Source.Dialog;
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class DialogManager
    {
        private static DialogManager instance;
        private static object instanceLock = new object();

        private WindowManager _windowManager;
        private ResourceManager _resourceManager;
        private GameStateManager _gameStateManager;
        private SoundManager _soundManager;
        private TextManager _textManager;
        private MazeWorldManager _mazeWorldManager;
        private Avatar _avatar;

        private Store _store;
        private bool _hasSellingItems = false;
        private TimedMessage _message = new TimedMessage(timeOut: 2000);
        private Dictionary<string, Item> _items;

        private ImageResource _buttonsImage;
        private ImageResource _buttonSelectedImage;
        private Button[] _buttons = new Button[3]
        {
            new Button(0, 120, 32, 32),
            new Button(0, 160, 32, 32),
            new Button(0, 200, 32, 32),
        };

        private DialogManager()
        {
        }

        public static DialogManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new DialogManager();
                        Console.WriteLine("DialogManager created");
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
            _avatar = Avatar.GetInstance();

            _buttonsImage = _resourceManager.LoadImage("Data/images/interface/dialog_buttons.png");
            _buttonSelectedImage = _resourceManager.LoadImage("Data/images/interface/select.png");

            LoadItems("Data/items.json");

            Console.WriteLine("DialogManager initialized");
        }

        public void LoadStore(string storeName)
        {
            // Loading store from file
            string fileName = string.Format("Data/stores/{0}.json", storeName);

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Error: Unable to load store {0} from {1}", storeName, fileName);
                return;
            }

            using (StreamReader streamReader = new(fileName))
            {
                string jsonData = streamReader.ReadToEnd();
                streamReader.Close();

                _store = JsonConvert.DeserializeObject<Store>(jsonData, new StringEnumConverter());
            }

            if (_store is null)
            {
                Console.WriteLine("Error: Unable to load broken store file {0}", fileName);
                return;
            }

            ResetButtons();
            UpdateButtons();

            _soundManager.PlayMusic(_store.Music);

            Console.WriteLine("Store {0} loaded from {1}", storeName, fileName);
        }

        public void LoadItems(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Error: Unable to load items file {0}", fileName);
                return;
            }

            using (StreamReader streamReader = new(fileName))
            {
                string jsonData = streamReader.ReadToEnd();
                streamReader.Close();

                _items = JsonConvert.DeserializeObject<Dictionary<string, Item>>(jsonData, new StringEnumConverter());
            }

            Console.WriteLine("Items loaded from {0}", fileName);
        }

        public bool GetItem(string name, out Item item)
        {
            if (_items.TryGetValue(name, out item))
            {
                return true;
            }

            return false;
        }

        public void ResetButtons()
        {
            _hasSellingItems = false;

            for (int i = 0; i < _buttons.Length; i++)
            {
                // Last button for Exit
                int lastButton = _buttons.Length - 1;
                _buttons[i].Action = (i == lastButton) ? DialogButtonAction.Exit : DialogButtonAction.None;
                _buttons[i].Selected = i == lastButton;
                _buttons[i].TextFirst = i == lastButton ? "Exit" : string.Empty;
                _buttons[i].TextSecond = string.Empty;
                _buttons[i].Enabled = i == lastButton;
            }
        }

        public void UpdateButtons()
        {
            _hasSellingItems = false;

            int linesCount = _store.Lines.Length > _buttons.Length ? _buttons.Length : _store.Lines.Length;
            for (int i = 0; i < linesCount; i++)
            {
                _buttons[i].Enabled = false;

                switch (_store.Lines[i].Type)
                {
                    case DialogType.Weapon:
                    case DialogType.Armor:
                        if (GetItem(_store.Lines[i].Value, out var item))
                        {
                            _hasSellingItems = true;
                            _buttons[i].Action = DialogButtonAction.Buy;
                            _buttons[i].TextFirst = string.Format("Buy {0}", item.Name);

                            if (_avatar.HasItem(item))
                            {
                                _buttons[i].TextSecond = "(You own this)";
                            }
                            else if (!_avatar.IsBetterItem(item))
                            {
                                _buttons[i].TextSecond = "(Yours is better)";
                            }
                            else
                            {
                                _buttons[i].TextSecond = string.Format("for {0} Gold", item.Gold);
                                _buttons[i].Enabled = true;
                            }
                        }

                        break;

                    case DialogType.Spell:
                        if (GetItem(_store.Lines[i].Value, out var spell))
                        {
                            _hasSellingItems = true;
                            _buttons[i].Action = DialogButtonAction.Buy;
                            _buttons[i].TextFirst = string.Format("Learn {0}", spell.Name);

                            if (_avatar.KnowsSpell(spell))
                            {
                                _buttons[i].TextSecond = "(You know this)";
                            }
                            else if (spell.Level > _avatar.SpellBookLevel + 1)
                            {
                                _buttons[i].TextSecond = "(Too advanced)";
                            }
                            else
                            {
                                _buttons[i].TextSecond = string.Format("for {0} Gold", spell.Gold);
                                _buttons[i].Enabled = true;
                            }
                        }

                        break;

                    case DialogType.Room:
                        _hasSellingItems = true;
                        _buttons[i].Action = DialogButtonAction.Buy;
                        _buttons[i].TextFirst = "Rent a room for the night";

                        if (_avatar.IsRested())
                        {
                            _buttons[i].TextSecond = "(You are well rested)";
                        }
                        else
                        {
                            _buttons[i].TextSecond = string.Format("for {0} Gold", _store.Lines[i].Cost);
                            _buttons[i].Enabled = true;
                        }

                        break;

                    case DialogType.Message:
                        _buttons[i].TextFirst = _store.Lines[i].MessageFirst;
                        _buttons[i].TextSecond = _store.Lines[i].MessageSecond;

                        break;
                }
            }
        }

        public void Update()
        {
            _message.Update();

            if (_windowManager.KeyPressed(InputKey.KEY_UP) ||
                _windowManager.KeyPressed(InputKey.KEY_DOWN))
            {
                var buttons = _buttons
                    .Where(x => !x.Action.Equals(DialogButtonAction.None))
                    .ToList();

                if (_windowManager.KeyPressed(InputKey.KEY_DOWN))
                {
                    buttons.Reverse();
                }

                foreach (var button in buttons)
                {
                    if (button.Selected &&
                        !button.Equals(buttons.First()))
                    {
                        button.Selected = false;
                        buttons[buttons.IndexOf(button) - 1].Selected = true;
                    }
                }
            }

            if (_windowManager.KeyPressed(InputKey.KEY_A))
            {
                for (int i = 0; i < _buttons.Length; i++)
                {
                    if (!_buttons[i].Selected ||
                        !_buttons[i].Enabled)
                    {
                        continue;
                    }

                    if (_buttons[i].Action.Equals(DialogButtonAction.Exit))
                    {
                        _message.Clear();
                        _gameStateManager.ChangeState(GameState.Explore);
                        return;
                    }

                    if (_buttons[i].Action.Equals(DialogButtonAction.Buy))
                    {
                        switch (_store.Lines[i].Type)
                        {
                            case DialogType.Weapon:
                            case DialogType.Armor:
                                if (GetItem(_store.Lines[i].Value, out var item))
                                {
                                    if (_avatar.Gold < item.Gold)
                                    {
                                        _message.Start("Your gold isn't enough!");
                                        return;
                                    }

                                    _avatar.AddGold(-item.Gold);

                                    _avatar.EquipItem(item);
                                    _message.Start(string.Format("Bought {0}", item.Name));
                                }

                                break;

                            case DialogType.Spell:
                                if (GetItem(_store.Lines[i].Value, out var spell))
                                {
                                    if (_avatar.Gold < spell.Gold)
                                    {
                                        _message.Start("Your gold isn't enough!");
                                        return;
                                    }

                                    _avatar.AddGold(-spell.Gold);

                                    _avatar.LearnSpell(spell);
                                    _message.Start(string.Format("Learned {0}", spell.Name));
                                }

                                break;

                            case DialogType.Room:
                                if (_avatar.Gold < _store.Lines[i].Cost)
                                {
                                    _message.Start("Your gold isn't enough!");
                                    return;
                                }

                                _avatar.AddGold(-_store.Lines[i].Cost);

                                _avatar.Sleep();
                                _message.Start("You have rested");

                                break;
                        }

                        UpdateButtons();
                        _soundManager.PlaySound(SoundFX.Coin);

                        return;
                    }
                }
            }

            if (_windowManager.KeyPressed(InputKey.KEY_B))
            {
                _message.Clear();
                _gameStateManager.ChangeState(GameState.Explore);
                return;
            }
        }

        public void Render()
        {
            _mazeWorldManager.RenderBackground(_store.BackgroundImage);

            _textManager.Render(_store.Name, 160, 4, TextJustify.JUSTIFY_CENTER);

            RenderButtons();
            RenderTexts();

            if (_hasSellingItems)
            {
                RenderGold();
            }

            _textManager.Render(_message.Text, 160, 80, TextJustify.JUSTIFY_CENTER);
        }

        private void RenderButtons()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i].Action == DialogButtonAction.None)
                {
                    continue;
                }

                if (_buttons[i].Selected)
                {
                    _buttonSelectedImage.Render(
                        0,
                        0,
                        _buttonSelectedImage.Width,
                        _buttonSelectedImage.Height,
                        _buttons[i].Position.X,
                        _buttons[i].Position.Y);
                }

                _buttonsImage.Render(
                    ((int)_buttons[i].Action - 1) * _buttons[i].Position.Width,
                    0,
                    _buttons[i].Position.Width,
                    _buttons[i].Position.Height,
                    _buttons[i].Position.X + 4,
                    _buttons[i].Position.Y + 4);
            }
        }

        private void RenderTexts()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                if (string.IsNullOrEmpty(_buttons[i].TextFirst) &&
                    string.IsNullOrEmpty(_buttons[i].TextSecond))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(_buttons[i].TextSecond))
                {
                    _textManager.Render(
                        _buttons[i].TextFirst,
                        _buttons[i].Position.X + (_buttons[i].Action.Equals(DialogButtonAction.None) ? 4 : 44),
                        _buttons[i].Position.Y + 12);

                    continue;
                }

                _textManager.Render(
                        _buttons[i].TextFirst,
                        _buttons[i].Position.X + (_buttons[i].Action.Equals(DialogButtonAction.None) ? 4 : 44),
                        _buttons[i].Position.Y + 2);
                _textManager.Render(
                        _buttons[i].TextSecond,
                        _buttons[i].Position.X + (_buttons[i].Action.Equals(DialogButtonAction.None) ? 4 : 44),
                        _buttons[i].Position.Y + 22);
            }
        }

        private void RenderGold()
        {
            _textManager.Render(string.Format("{0} Gold", _avatar.Gold), 316, 220, TextJustify.JUSTIFY_RIGHT);
        }

        private void Test()
        {
            Store store = new Store
            {
                Name = "Cedar Arms",
                BackgroundImage = 3,
                Music = "m31",
                Lines = new DialogLine[2]
                {
                    new DialogLine
                    {
                        Type = DialogType.Weapon,
                        Value = "2",
                        MessageFirst = string.Empty,
                        MessageSecond = string.Empty,
                    },
                    new DialogLine
                    {
                        Type = DialogType.Weapon,
                        Value = "3",
                        MessageFirst = "msg1",
                        MessageSecond = "msg2",
                    },
                },
            };

            using (StreamWriter streamWriter = new("StoreTest.json"))
            {
                string storeData = JsonConvert.SerializeObject(store, new StringEnumConverter());
                streamWriter.Write(storeData);
            }
        }
    }
}
