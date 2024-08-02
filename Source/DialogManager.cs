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

        private Shop _shop;
        private ImageResource _buttons;
        private int _buttonSelected = 2;

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

            _buttons = _resourceManager.LoadImage("Data/images/interface/dialog_buttons.png");

            Console.WriteLine("DialogManager initialized");

            Test();
        }

        public void LoadShop(string shopName)
        {
            string fileName = string.Format("Data/shop/{0}.json", shopName);

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Error: Unable to load Shop {0} from {1}", shopName, fileName);
                return;
            }

            using (StreamReader streamReader = new(fileName))
            {
                string jsonData = streamReader.ReadToEnd();
                streamReader.Close();

                _shop = JsonConvert.DeserializeObject<Shop>(jsonData, new StringEnumConverter());
            }

            _soundManager.PlayMusic(_shop.Music);

            Console.WriteLine("Shop {0} loaded from {1}", shopName, fileName);
        }

        public void Update()
        {
            if (_windowManager.KeyPressed(InputKey.KEY_B))
            {
                _gameStateManager.ChangeState(GameState.Explore);
            }
        }

        public void Render()
        {
            _mazeWorldManager.RenderBackground(_shop.BackgroundImage);

            _textManager.Render(_shop.Name, 160, 4, TextJustify.JUSTIFY_CENTER);
        }

        private void Test()
        {
            Shop shop = new Shop
            {
                Name = "Cedar Arms",
                BackgroundImage = 3,
                Music = "m31",
                Items = new DialogItem[2]
                {
                    new DialogItem
                    {
                        Type = DialogType.Weapon,
                        Value = 2,
                        MessageFirst = string.Empty,
                        MessageSecond = string.Empty,
                    },
                    new DialogItem
                    {
                        Type = DialogType.Weapon,
                        Value = 3,
                        MessageFirst = "msg1",
                        MessageSecond = "msg2",
                    },
                },
            };

            using (StreamWriter streamWriter = new("ShopTest.json"))
            {
                string shopData = JsonConvert.SerializeObject(shop, new StringEnumConverter());
                streamWriter.Write(shopData);
            }
        }
    }
}
