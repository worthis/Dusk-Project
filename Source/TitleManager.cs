namespace DuskProject.Source
{
    using DuskProject.Source.Resources;

    public enum TitleState : byte
    {
        Main = 0,
        Options = 1,
        Quit = 2,
    }

    public class TitleManager
    {
        private static TitleManager instance;
        private static object instanceLock = new object();

        private GameStateManager _gameStateManager;
        private WindowManager _windowManager;
        private ResourceManager _resourceManager;
        private TextManager _textManager;

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

            _menuCount = Enum.GetValues(typeof(TitleState)).Length;

            _imageBackground = _resourceManager.LoadImage("Data/images/backgrounds/title.png");

            Console.WriteLine("TitleManager initialized");
        }

        public void Update()
        {
            if (_windowManager.KeyPressed(InputKeys.KEY_UP))
            {
                _menuSelected--;
                if (_menuSelected < 0)
                {
                    _menuSelected = 0;
                }
            }

            if (_windowManager.KeyPressed(InputKeys.KEY_DOWN))
            {
                _menuSelected++;
                if (_menuSelected >= _menuCount)
                {
                    _menuSelected = _menuCount - 1;
                }
            }

            if (_windowManager.KeyPressed(InputKeys.KEY_A))
            {
                switch (_menuSelected)
                {
                    case 0:
                        _gameStateManager.ChangeState(GameState.Explore);
                        break;

                    case 1:

                        break;

                    case 2:
                        _gameStateManager.RegisterQuit();
                        break;
                }
            }
        }

        public void Render()
        {
            _windowManager.Draw(_imageBackground);

            switch (_state)
            {
                case TitleState.Main:
                    {
                        _textManager.Render(GetMenuItemText("START", 0, _menuSelected), 160, 100, TextJustify.JUSTIFY_CENTER);
                        _textManager.Render(GetMenuItemText("OPTIONS", 1, _menuSelected), 160, 100 + 16, TextJustify.JUSTIFY_CENTER);
                        _textManager.Render(GetMenuItemText("QUIT", 2, _menuSelected), 160, 100 + 16 + 16, TextJustify.JUSTIFY_CENTER);

                        _textManager.Render("by Clint Bellanger 2013", 160, 200, TextJustify.JUSTIFY_CENTER);
                        _textManager.Render("ft. music by Yubatake", 160, 200 + 16, TextJustify.JUSTIFY_CENTER);
                    }

                    break;

                case TitleState.Options:
                    {
                    }

                    break;
            }

            TextManager.GetInstance().Render("Hello, traveller!", 160, 60, TextJustify.JUSTIFY_CENTER);
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
