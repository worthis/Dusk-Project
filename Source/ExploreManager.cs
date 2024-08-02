namespace DuskProject.Source
{
    using DuskProject.Source.Enums;
    using DuskProject.Source.Maze;

    public class ExploreManager
    {
        private static ExploreManager instance;
        private static object instanceLock = new object();

        private GameStateManager _gameStateManager;
        private TextManager _textManager;
        private MazeWorldManager _mazeWorldManager;
        private DialogManager _dialogManager;
        private Avatar _avatar;

        private string _message = string.Empty;

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
            _gameStateManager = GameStateManager.GetInstance();
            _textManager = TextManager.GetInstance();
            _mazeWorldManager = MazeWorldManager.GetInstance();
            _dialogManager = DialogManager.GetInstance();
            _avatar = Avatar.GetInstance();

            Console.WriteLine("ExploreManager initialized");
        }

        public void Update()
        {
            _message = string.Empty;

            _avatar.Update();

            if (_avatar.Moved)
            {
                // Check exit portals
                if (_mazeWorldManager.CheckPortals(_avatar.PosX, _avatar.PosY, out MazePortal mazePortal))
                {
                    _avatar.PosX = mazePortal.DestX;
                    _avatar.PosY = mazePortal.DestY;

                    _mazeWorldManager.LoadMazeWorld(mazePortal.Destination);

                    _message = _mazeWorldManager.MazeWorldName;

                    return;
                }

                // Check shop entrance
                if (_mazeWorldManager.CheckShops(_avatar.PosX, _avatar.PosY, out ShopPortal shopPortal))
                {
                    _avatar.PosX = shopPortal.DestX;
                    _avatar.PosY = shopPortal.DestY;

                    _dialogManager.LoadShop(shopPortal.ShopId);
                    _gameStateManager.ChangeState(GameState.Dialog);

                    return;
                }

                // Special scripts
                // Encounters
            }
        }

        public void Render()
        {
            // Maze Cell
            _mazeWorldManager.RenderBackground(_avatar.Facing);
            _mazeWorldManager.Render(_avatar.PosX, _avatar.PosY, _avatar.Facing);

            // UI
            // Compass
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

            // Minimap
            // Messages
            _textManager.Render(_message, 160, 200, TextJustify.JUSTIFY_CENTER);
        }
    }
}
