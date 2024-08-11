namespace DuskProject.Source.UI
{
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;

    public class Minimap
    {
        private MinimapTileType[,] _tileTypes;

        private Button[] _tiles = new Button[Enum.GetValues(typeof(MinimapTileType)).Length];
        private Button[] _heroCursor = new Button[Enum.GetValues(typeof(AvatarFacing)).Length];

        public Minimap(int width, int height, ImageResource tileImage, ImageResource heroImage)
        {
            if (width <= 0 ||
                height <= 0 ||
                tileImage is null ||
                heroImage is null)
            {
                Console.WriteLine("Error: Can not create minimap");
                return;
            }

            // Note: x,y flipped to ease map making
            _tileTypes = new MinimapTileType[width, height];

            _tiles[(int)MinimapTileType.Blocked] = new Button(0, 0, 6, 6, tileImage, 0 * 6, 0);
            _tiles[(int)MinimapTileType.Walkable] = new Button(0, 0, 6, 6, tileImage, 1 * 6, 0);
            _tiles[(int)MinimapTileType.Store] = new Button(0, 0, 6, 6, tileImage, 2 * 6, 0);
            _tiles[(int)MinimapTileType.Portal] = new Button(0, 0, 6, 6, tileImage, 3 * 6, 0);

            _heroCursor[(int)AvatarFacing.West] = new Button(0, 0, 6, 6, heroImage, 0 * 6, 0);
            _heroCursor[(int)AvatarFacing.North] = new Button(0, 0, 6, 6, heroImage, 1 * 6, 0);
            _heroCursor[(int)AvatarFacing.East] = new Button(0, 0, 6, 6, heroImage, 2 * 6, 0);
            _heroCursor[(int)AvatarFacing.South] = new Button(0, 0, 6, 6, heroImage, 3 * 6, 0);
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get => _tileTypes.GetLength(0); }

        public int Height { get => _tileTypes.GetLength(1); }

        public bool Enabled { get; set; } = false;

        public void SetTileType(int posX, int posY, MinimapTileType minimapTileType)
        {
            if (!CheckBounds(posX, posY))
            {
                return;
            }

            _tileTypes[posX, posY] = minimapTileType;
        }

        public MinimapTileType GetTileType(int posX, int posY)
        {
            if (!CheckBounds(posX, posY))
            {
                return MinimapTileType.Blocked;
            }

            return _tileTypes[posX, posY];
        }

        public void Render()
        {
            if (!Enabled)
            {
                return;
            }

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    var tileType = _tileTypes[i, j];
                    _tiles[(int)tileType].Render((i * 6) + X, (j * 6) + Y);
                }
            }
        }

        public void RenderHero(int posX, int posY, AvatarFacing avatarFacing)
        {
            if (!Enabled)
            {
                return;
            }

            _heroCursor[(int)avatarFacing].Render((posX * 6) + X, (posY * 6) + Y);
        }

        private bool CheckBounds(int posX, int posY)
        {
            if (posX >= 0 &&
                posX < Width &&
                posY >= 0 &&
                posY < Height)
            {
                return true;
            }

            return false;
        }
    }
}
