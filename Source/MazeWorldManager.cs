namespace DuskProject.Source
{
    using DuskProject.Source.Enums;
    using DuskProject.Source.Maze;
    using DuskProject.Source.Resources;
    using Newtonsoft.Json;

    public class MazeWorldManager
    {
        private static MazeWorldManager instance;
        private static object instanceLock = new object();

        private readonly TileLayout[] _tileLayouts =
        {
            new() { Width = 160, Height = 240, SrcX = 0, SrcY = 0, DstX = 0, DstY = 0 },
            new() { Width = 160, Height = 240, SrcX = 160, SrcY = 0, DstX = 160, DstY = 0 },
            new() { Width = 160, Height = 240, SrcX = 320, SrcY = 0, DstX = 0, DstY = 0 },
            new() { Width = 160, Height = 240, SrcX = 480, SrcY = 0, DstX = 160, DstY = 0 },
            new() { Width = 320, Height = 240, SrcX = 640, SrcY = 0, DstX = 0, DstY = 0 },
            new() { Width = 160, Height = 240, SrcX = 960, SrcY = 0, DstX = 0, DstY = 0 },
            new() { Width = 160, Height = 240, SrcX = 1120, SrcY = 0, DstX = 160, DstY = 0 },
            new() { Width = 160, Height = 240, SrcX = 0, SrcY = 240, DstX = 0, DstY = 0 },
            new() { Width = 160, Height = 240, SrcX = 160, SrcY = 240, DstX = 160, DstY = 0 },
            new() { Width = 320, Height = 240, SrcX = 320, SrcY = 240, DstX = 0, DstY = 0 },
            new() { Width = 160, Height = 240, SrcX = 640, SrcY = 240, DstX = 0, DstY = 0 },
            new() { Width = 160, Height = 240, SrcX = 800, SrcY = 240, DstX = 160, DstY = 0 },
            new() { Width = 320, Height = 240, SrcX = 960, SrcY = 240, DstX = 0, DstY = 0 },
        };

        private ResourceManager _resourceManager;
        private SoundManager _soundManager;

        private Tile[] _tileSet;
        private ImageResource[] _backgrounds;
        private MazeWorld _mazeWorld;

        private MazeWorldManager()
        {
        }

        public int TileSetRenderOffsetX { get; set; } = 0;

        public int TileSetRenderOffsetY { get; set; } = 0;

        public string MazeWorldName { get => _mazeWorld.Name; }

        public string MazeWorldId { get => _mazeWorld.Id; }

        public List<string> Enemies { get => _mazeWorld.Enemies; }

        public static MazeWorldManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new MazeWorldManager();
                        Console.WriteLine("MazeWorldManager created");
                    }
                }
            }

            return instance;
        }

        public void Init()
        {
            _resourceManager = ResourceManager.GetInstance();
            _soundManager = SoundManager.GetInstance();

            LoadTiles();
            LoadBackgrounds();

            Console.WriteLine("MazeWorldManager initialized");
        }

        public void LoadMazeWorld(string mazeWorldName)
        {
            string fileName = string.Format("Data/maze/{0}.json", mazeWorldName);

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Error: Unable to load MazeWorld {0} from {1}", mazeWorldName, fileName);
                return;
            }

            using (StreamReader streamReader = new(fileName))
            {
                string jsonData = streamReader.ReadToEnd();
                streamReader.Close();

                _mazeWorld = JsonConvert.DeserializeObject<MazeWorld>(jsonData);
                _mazeWorld.Id = mazeWorldName;
            }

            if (_mazeWorld.Width != _mazeWorld.Tiles.GetLength(1) &&
                _mazeWorld.Height != _mazeWorld.Tiles.GetLength(0))
            {
                Console.WriteLine(
                    "Error: MazeWorld {0} dimensions {1}x{2} do not match the tiles dimensions {3}x{4}",
                    MazeWorldName,
                    _mazeWorld.Width,
                    _mazeWorld.Height,
                    _mazeWorld.Tiles.GetLength(0),
                    _mazeWorld.Tiles.GetLength(1));
            }

            TileSetRenderOffsetX = 0;
            TileSetRenderOffsetY = 0;

            _soundManager.PlayMusic(_mazeWorld.Music);

            Console.WriteLine("MazeWorld {0} loaded from {1}", mazeWorldName, fileName);
        }

        public void InitScriptedEvents(Predicate<string> has)
        {
            _mazeWorld.MessagePoints.RemoveAll(x => has(x.UniqueId));
            _mazeWorld.Chests.RemoveAll(x => has(x.UniqueId));
            _mazeWorld.ScriptedEnemies.RemoveAll(x => has(x.UniqueId));
            _mazeWorld.ScriptedTiles.RemoveAll(x => has(x.UniqueId));

            foreach (var item in _mazeWorld.Chests)
            {
                SetTile(item.X, item.Y, item.ClosedTileId);
            }

            foreach (var item in _mazeWorld.ScriptedTiles)
            {
                SetTile(item.X, item.Y, item.BeforeTileId);
            }
        }

        public bool CheckPortals(int posX, int posY, out MazePortal mazePortal)
        {
            mazePortal = _mazeWorld.Portals
                .Where(x => x.CheckEnter(posX, posY))
                .FirstOrDefault();

            return mazePortal is not null;
        }

        public bool CheckStores(int posX, int posY, out StorePortal storePortal)
        {
            storePortal = _mazeWorld.Stores
                .Where(x => x.CheckEnter(posX, posY))
                .FirstOrDefault();

            return storePortal is not null;
        }

        public bool CheckMessagePoints(int posX, int posY, out MessagePoint messagePoint)
        {
            messagePoint = _mazeWorld.MessagePoints
                .Where(x => x.CheckEnter(posX, posY))
                .FirstOrDefault();

            return messagePoint is not null;
        }

        public bool CheckRestPoints(int posX, int posY, out RestPoint restPoint)
        {
            restPoint = _mazeWorld.RestPoints
                .Where(x => x.CheckEnter(posX, posY))
                .FirstOrDefault();

            return restPoint is not null;
        }

        public bool CheckChests(int posX, int posY, out ChestPoint chestPoint)
        {
            chestPoint = _mazeWorld.Chests
                .Where(x => x.CheckEnter(posX, posY))
                .FirstOrDefault();

            return chestPoint is not null;
        }

        public bool CheckScriptedEnemies(int posX, int posY, out ScriptedEnemy scriptedEnemy)
        {
            scriptedEnemy = _mazeWorld.ScriptedEnemies
                .Where(x => x.CheckEnter(posX, posY))
                .FirstOrDefault();

            return scriptedEnemy is not null;
        }

        public bool CheckScriptedTiles(int posX, int posY, out ScriptedTile scriptedTile)
        {
            scriptedTile = _mazeWorld.ScriptedTiles
                .Where(x => x.CheckEnter(posX, posY))
                .FirstOrDefault();

            return scriptedTile is not null;
        }

        public void Render(int posX, int posY, AvatarFacing avatarFacing)
        {
            if (avatarFacing == AvatarFacing.North)
            {
                // Back Row
                RenderTile(posX - 2, posY - 2, 0);
                RenderTile(posX + 2, posY - 2, 1);
                RenderTile(posX - 1, posY - 2, 2);
                RenderTile(posX + 1, posY - 2, 3);
                RenderTile(posX, posY - 2, 4);

                // Middle Row
                RenderTile(posX - 2, posY - 1, 5);
                RenderTile(posX + 2, posY - 1, 6);
                RenderTile(posX - 1, posY - 1, 7);
                RenderTile(posX + 1, posY - 1, 8);
                RenderTile(posX, posY - 1, 9);

                // Front Row
                RenderTile(posX - 1, posY, 10);
                RenderTile(posX + 1, posY, 11);
                RenderTile(posX, posY, 12);

                return;
            }

            if (avatarFacing == AvatarFacing.South)
            {
                // Back Row
                RenderTile(posX + 2, posY + 2, 0);
                RenderTile(posX - 2, posY + 2, 1);
                RenderTile(posX + 1, posY + 2, 2);
                RenderTile(posX - 1, posY + 2, 3);
                RenderTile(posX, posY + 2, 4);

                // Middle Row
                RenderTile(posX + 2, posY + 1, 5);
                RenderTile(posX - 2, posY + 1, 6);
                RenderTile(posX + 1, posY + 1, 7);
                RenderTile(posX - 1, posY + 1, 8);
                RenderTile(posX, posY + 1, 9);

                // Front Row
                RenderTile(posX + 1, posY, 10);
                RenderTile(posX - 1, posY, 11);
                RenderTile(posX, posY, 12);

                return;
            }

            if (avatarFacing == AvatarFacing.West)
            {
                // Back Row
                RenderTile(posX - 2, posY + 2, 0);
                RenderTile(posX - 2, posY - 2, 1);
                RenderTile(posX - 2, posY + 1, 2);
                RenderTile(posX - 2, posY - 1, 3);
                RenderTile(posX - 2, posY, 4);

                // Middle Row
                RenderTile(posX - 1, posY + 2, 5);
                RenderTile(posX - 1, posY - 2, 6);
                RenderTile(posX - 1, posY + 1, 7);
                RenderTile(posX - 1, posY - 1, 8);
                RenderTile(posX - 1, posY, 9);

                // Front Row
                RenderTile(posX, posY + 1, 10);
                RenderTile(posX, posY - 1, 11);
                RenderTile(posX, posY, 12);

                return;
            }

            if (avatarFacing == AvatarFacing.East)
            {
                // Back Row
                RenderTile(posX + 2, posY - 2, 0);
                RenderTile(posX + 2, posY + 2, 1);
                RenderTile(posX + 2, posY - 1, 2);
                RenderTile(posX + 2, posY + 1, 3);
                RenderTile(posX + 2, posY, 4);

                // Middle Row
                RenderTile(posX + 1, posY - 2, 5);
                RenderTile(posX + 1, posY + 2, 6);
                RenderTile(posX + 1, posY - 1, 7);
                RenderTile(posX + 1, posY + 1, 8);
                RenderTile(posX + 1, posY, 9);

                // Front Row
                RenderTile(posX, posY - 1, 10);
                RenderTile(posX, posY + 1, 11);
                RenderTile(posX, posY, 12);

                return;
            }
        }

        public void RenderBackground(AvatarFacing avatarFacing)
        {
            if (_mazeWorld is null)
            {
                return;
            }

            _backgrounds[_mazeWorld.BackgroundImage].Render();
        }

        public void RenderBackground(int backgroundId)
        {
            if (backgroundId < 0 ||
                backgroundId >= _backgrounds.Length)
            {
                return;
            }

            _backgrounds[backgroundId].Render();
        }

        public Tile GetTile(int posX, int posY)
        {
            int tileId = GetTileId(posX, posY);

            if (tileId <= 0 ||
                tileId >= _tileSet.Length)
            {
                return _tileSet[0];
            }

            return _tileSet[tileId];
        }

        public void SetTile(int posX, int posY, int tileId)
        {
            if (!CheckBounds(posX, posY))
            {
                return;
            }

            // Note: x,y flipped to ease map making
            _mazeWorld.Tiles[posY, posX] = tileId;
        }

        private bool CheckBounds(int posX, int posY)
        {
            if (_mazeWorld is null)
            {
                Console.WriteLine("Error: Unable to check bounds for ({0};{1})", posX, posY);
                return false;
            }

            // Note: x,y flipped to ease map making
            if (posX >= 0 &&
                posX < _mazeWorld.Width &&
                posY >= 0 &&
                posY < _mazeWorld.Height)
            {
                return true;
            }

            return false;
        }

        private void RenderTile(int posX, int posY, int position)
        {
            int tileId = GetTileId(posX, posY);

            if (tileId <= 0 ||
                tileId >= _tileSet.Length)
            {
                return;
            }

            _tileSet[tileId].Image.Render(
                _tileLayouts[position].SrcX,
                _tileLayouts[position].SrcY,
                _tileLayouts[position].Width,
                _tileLayouts[position].Height,
                _tileLayouts[position].DstX + TileSetRenderOffsetX,
                _tileLayouts[position].DstY = TileSetRenderOffsetY);
        }

        private void LoadTiles()
        {
            _tileSet = new Tile[22];
            _tileSet[0] = new Tile(null, false);
            _tileSet[1] = new Tile(_resourceManager.LoadImage("Data/images/tiles/dungeon_floor.png"), true);
            _tileSet[2] = new Tile(_resourceManager.LoadImage("Data/images/tiles/dungeon_wall.png"), false);
            _tileSet[3] = new Tile(_resourceManager.LoadImage("Data/images/tiles/dungeon_door.png"), true);
            _tileSet[4] = new Tile(_resourceManager.LoadImage("Data/images/tiles/pillar_exterior.png"), false);
            _tileSet[5] = new Tile(_resourceManager.LoadImage("Data/images/tiles/dungeon_ceiling.png"), true);
            _tileSet[6] = new Tile(_resourceManager.LoadImage("Data/images/tiles/grass.png"), true);
            _tileSet[7] = new Tile(_resourceManager.LoadImage("Data/images/tiles/pillar_interior.png"), false);
            _tileSet[8] = new Tile(_resourceManager.LoadImage("Data/images/tiles/chest_interior.png"), true);
            _tileSet[9] = new Tile(_resourceManager.LoadImage("Data/images/tiles/chest_exterior.png"), true);
            _tileSet[10] = new Tile(_resourceManager.LoadImage("Data/images/tiles/medieval_house.png"), false);
            _tileSet[11] = new Tile(_resourceManager.LoadImage("Data/images/tiles/medieval_door.png"), true);
            _tileSet[12] = new Tile(_resourceManager.LoadImage("Data/images/tiles/tree_evergreen.png"), false);
            _tileSet[13] = new Tile(_resourceManager.LoadImage("Data/images/tiles/grave_cross.png"), false);
            _tileSet[14] = new Tile(_resourceManager.LoadImage("Data/images/tiles/grave_stone.png"), false);
            _tileSet[15] = new Tile(_resourceManager.LoadImage("Data/images/tiles/water.png"), false);
            _tileSet[16] = new Tile(_resourceManager.LoadImage("Data/images/tiles/skull_pile.png"), false);
            _tileSet[17] = new Tile(_resourceManager.LoadImage("Data/images/tiles/hay_pile.png"), true);
            _tileSet[18] = new Tile(_resourceManager.LoadImage("Data/images/tiles/locked_door.png"), false);
            _tileSet[19] = new Tile(_resourceManager.LoadImage("Data/images/tiles/death_speaker.png"), true);
            _tileSet[20] = new Tile(_resourceManager.LoadImage("Data/images/tiles/wood_floor.png"), true);
            _tileSet[21] = new Tile(_resourceManager.LoadImage("Data/images/tiles/template_floor.png"), true);
        }

        private void LoadBackgrounds()
        {
            _backgrounds = new ImageResource[4];
            _backgrounds[0] = _resourceManager.LoadImage("Data/images/backgrounds/black.png");
            _backgrounds[1] = _resourceManager.LoadImage("Data/images/backgrounds/nightsky.png");
            _backgrounds[2] = _resourceManager.LoadImage("Data/images/backgrounds/tempest.png");
            _backgrounds[3] = _resourceManager.LoadImage("Data/images/backgrounds/interior.png");
        }

        private int GetTileId(int posX, int posY)
        {
            if (!CheckBounds(posX, posY))
            {
                return 0;
            }

            // Note: x,y flipped to ease map making
            return _mazeWorld.Tiles[posY, posX];
        }

        private void Test()
        {
            MazeWorld mazeWorld = new MazeWorld();
            mazeWorld.Name = "Monastery Trail";
            mazeWorld.Id = "Monastery Trail ID";
            mazeWorld.BackgroundImage = 1;
            mazeWorld.Music = "m31";

            mazeWorld.Width = 14;
            mazeWorld.Height = 16;
            mazeWorld.Tiles = new int[,]
            {
                { 0, 12, 12, 2, 0, 0, 0, 0, 0, 2, 12, 0, 0, 0 },
                { 12, 12, 12, 2, 2, 2, 3, 2, 2, 2, 12, 0, 0, 0 },
                { 12, 12, 9, 6, 12, 6, 1, 6, 6, 12, 12, 12, 0, 0 },
                { 12, 12, 12, 6, 6, 6, 1, 12, 6, 6, 12, 12, 0, 0 },
                { 0, 12, 12, 12, 12, 6, 1, 6, 6, 12, 12, 12, 0, 0 },
                { 0, 0, 12, 12, 12, 12, 6, 6, 12, 12, 12, 12, 0, 0 },
                { 0, 0, 0, 12, 12, 6, 6, 6, 6, 12, 12, 12, 12, 0 },
                { 0, 0, 12, 12, 12, 6, 6, 12, 6, 6, 6, 12, 12, 0 },
                { 0, 12, 12, 12, 6, 6, 12, 12, 12, 6, 12, 12, 12, 0 },
                { 0, 12, 12, 6, 6, 6, 12, 12, 6, 6, 6, 12, 12, 0 },
                { 0, 12, 12, 12, 6, 6, 6, 6, 6, 6, 12, 12, 12, 0 },
                { 0, 0, 12, 12, 6, 6, 6, 12, 6, 6, 6,  12, 12, 12 },
                { 0, 0, 12, 12, 12, 6, 12, 12, 12, 6, 1, 6, 12, 12 },
                { 0, 0, 0, 12, 10, 11, 12, 12, 12, 12, 1, 6, 12, 12 },
                { 0, 0, 0, 0, 12, 12, 12, 12, 12, 6, 1, 6, 12, 12 },
                { 0, 0, 0, 0, 0, 0, 0, 12, 12, 2, 3, 2, 12, 12 },
            };

            mazeWorld.Portals = new();
            mazeWorld.Portals.Add(new MazePortal
            {
                X = 6,
                Y = 1,
                Destination = "1",
                DestX = 4,
                DestY = 9,
            });
            mazeWorld.Portals.Add(new MazePortal
            {
                X = 10,
                Y = 15,
                Destination = "5",
                DestX = 3,
                DestY = 1,
            });

            mazeWorld.Enemies = new();
            mazeWorld.Enemies.Add("ENEMY_SHADOW_TENDRILS");
            mazeWorld.Enemies.Add("ENEMY_IMP");
            mazeWorld.Enemies.Add("ENEMY_SHADOW_SOUL");

            mazeWorld.Stores = new();
            mazeWorld.Stores.Add(new StorePortal
            {
                X = 5,
                Y = 13,
                Store = "4",
                DestX = 5,
                DestY = 12,
            });

            using (StreamWriter streamWriter = new("mazeWorldTest.json"))
            {
                string mazeWorldData = JsonConvert.SerializeObject(mazeWorld);
                streamWriter.Write(mazeWorldData);
            }
        }
    }
}
