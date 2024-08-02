namespace DuskProject.Source
{
    using DuskProject.Source.Dialog;
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

        private TimedMessage _message = new TimedMessage(timeOut: 2500);

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
            _avatar.Update();
            _message.Update();

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
            _textManager.Render(_message.Text, 160, 200, TextJustify.JUSTIFY_CENTER);
        }
    }
}
