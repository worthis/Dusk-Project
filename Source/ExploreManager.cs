namespace DuskProject.Source
{
    using DuskProject.Source.MazeObjects;

    public class ExploreManager
    {
        private static ExploreManager instance;
        private static object instanceLock = new object();

        private TextManager _textManager;
        private MazeWorldManager _mazeWorldManager;
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
            _textManager = TextManager.GetInstance();
            _mazeWorldManager = MazeWorldManager.GetInstance();
            _avatar = Avatar.GetInstance();

            Console.WriteLine("ExploreManager initialized");
        }

        public void Update()
        {
            _message = string.Empty;

            _avatar.Update();

            if (_avatar.Moved)
            {
                if (_mazeWorldManager.CheckPortals(_avatar.PosX, _avatar.PosY, out MazePortal mazePortal))
                {
                    _avatar.PosX = mazePortal.DestX;
                    _avatar.PosY = mazePortal.DestY;

                    _mazeWorldManager.LoadMazeWorld(mazePortal.Destination);

                    _message = _mazeWorldManager.MazeWorldName;
                }

                // Check exit
                // Check shop
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
