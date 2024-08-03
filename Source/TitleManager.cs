namespace DuskProject.Source
{
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;

    public class TitleManager
    {
        private static TitleManager instance;
        private static object instanceLock = new object();

        private GameStateManager _gameStateManager;
        private WindowManager _windowManager;
        private ResourceManager _resourceManager;
        private TextManager _textManager;
        private ExploreManager _exploreManager;

        private TitleState _state = TitleState.Main;
        private ImageResource _imageBackground;
        private int _menuSelected = 0;
        private int _menuCount;

        private TitleManager()
        {
        }

        public static TitleManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new TitleManager();
                        Console.WriteLine("TitleManager created");
                    }
                }
            }

            return instance;
        }

        public void Init()
        {
            _gameStateManager = GameStateManager.GetInstance();
            _windowManager = WindowManager.GetInstance();
            _resourceManager = ResourceManager.GetInstance();
            _textManager = TextManager.GetInstance();
            _exploreManager = ExploreManager.GetInstance();

            _menuCount = Enum.GetValues(typeof(TitleState)).Length;

            _imageBackground = _resourceManager.LoadImage("Data/images/backgrounds/title.png");

            Console.WriteLine("TitleManager initialized");
        }

        public void Update()
        {
            if (_windowManager.KeyPressed(InputKey.KEY_UP))
            {
                _menuSelected--;
                if (_menuSelected < 0)
                {
                    _menuSelected = 0;
                }
            }

            if (_windowManager.KeyPressed(InputKey.KEY_DOWN))
            {
                _menuSelected++;
                if (_menuSelected >= _menuCount)
                {
                    _menuSelected = _menuCount - 1;
                }
            }

            if (_windowManager.KeyPressed(InputKey.KEY_A))
            {
                switch (_menuSelected)
                {
                    // Start
                    case 0:
                        _exploreManager.Start();
                        _gameStateManager.ChangeState(GameState.Explore);
                        break;

                    // Options
                    case 1:

                        break;

                    // Quit
                    case 2:
                        _gameStateManager.RegisterQuit();
                        break;
                }
            }
        }

        public void Render()
        {
            _imageBackground.Render();

            switch (_state)
            {
                case TitleState.Main:
                    {
                        _textManager.Render(GetMenuItemText("START", 0, _menuSelected), 160, 100, TextJustify.JUSTIFY_CENTER);
                        _textManager.Render(GetMenuItemText("OPTIONS", 1, _menuSelected), 160, 100 + 16, TextJustify.JUSTIFY_CENTER);
                        _textManager.Render(GetMenuItemText("QUIT", 2, _menuSelected), 160, 100 + 16 + 16, TextJustify.JUSTIFY_CENTER);

                        _textManager.Render("by Worthis, 2024", 160, 200, TextJustify.JUSTIFY_CENTER);
                        _textManager.Render("ft. music by Yubatake", 160, 200 + 16, TextJustify.JUSTIFY_CENTER);
                    }

                    break;

                case TitleState.Options:
                    {
                    }

                    break;
            }
        }

        private static string GetMenuItemText(string menuItemText, int menuItemPos, int menuSelected)
        {
            if (menuSelected == menuItemPos)
            {
                return string.Format("[ {0} ]", menuItemText);
            }

            return menuItemText;
        }
    }
}
