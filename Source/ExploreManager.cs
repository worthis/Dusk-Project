namespace DuskProject.Source
{
    public class ExploreManager
    {
        private static ExploreManager instance;
        private static object instanceLock = new object();

        private TextManager _textManager;
        private MazeWorldManager _mazeWorldManager;

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

            _mazeWorldManager.LoadMazeWorld("Data/maze/3-meditation-point.json");
        }

        public void Update()
        {
        }

        public void Render()
        {
            // Maze Cell
            _mazeWorldManager.Render(2, 4, AvatarFacing.North);

            // UI
            _textManager.Render("NORTH", 160, 4, TextJustify.JUSTIFY_CENTER);
        }
    }
}
