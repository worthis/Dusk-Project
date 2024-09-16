namespace DuskProject.Source;

using DuskProject.Source.Enums;
using DuskProject.Source.Resources;
using DuskProject.Source.UI;
using DuskProject.Source.World;
using Newtonsoft.Json;

public class WorldManager
{
    private static readonly object InstanceLock = new object();
    private static WorldManager instance;

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
    private WorldData _world;

    private Minimap _minimap;

    private WorldManager()
    {
        Console.WriteLine("WorldManager created");
    }

    public static WorldManager Instance
    {
        get
        {
            lock (InstanceLock)
            {
                return instance ??= new WorldManager();
            }
        }
    }

    public int TileSetRenderOffsetX { get; set; } = 0;

    public int TileSetRenderOffsetY { get; set; } = 0;

    public int Width { get => _world.Width; }

    public int Height { get => _world.Height; }

    public string WorldId { get => _world.Id; }

    public string WorldName { get => _world.Name; }

    public List<string> Enemies { get => _world.Enemies; }

    public Minimap Minimap { get => _minimap; }

    public void Init()
    {
        _resourceManager = ResourceManager.Instance;
        _soundManager = SoundManager.Instance;

        LoadTiles();
        LoadBackgrounds();

        Console.WriteLine("WorldManager initialized");
    }

    public void LoadWorld(string worldName)
    {
        string fileName = string.Format("Data/maze/{0}.json", worldName);

        if (!File.Exists(fileName))
        {
            Console.WriteLine("Error: Unable to load World {0} from {1}", worldName, fileName);
            return;
        }

        using (StreamReader streamReader = new(fileName))
        {
            string jsonData = streamReader.ReadToEnd();
            streamReader.Close();

            _world = JsonConvert.DeserializeObject<WorldData>(jsonData);
            _world.Id = worldName;
        }

        if (_world.Width != _world.Tiles.GetLength(1) &&
            _world.Height != _world.Tiles.GetLength(0))
        {
            Console.WriteLine(
                "Error: World {0} dimensions {1}x{2} do not match the tiles dimensions {3}x{4}",
                WorldName,
                _world.Width,
                _world.Height,
                _world.Tiles.GetLength(0),
                _world.Tiles.GetLength(1));
        }

        TileSetRenderOffsetX = 0;
        TileSetRenderOffsetY = 0;

        LoadMinimap();

        _soundManager.PlayMusic(_world.Music);

        Console.WriteLine("World {0} loaded from {1}", worldName, fileName);
    }

    public void InitScriptedEvents(Predicate<string> has)
    {
        _world.MessagePoints.RemoveAll(x => has(x.UniqueId));
        _world.Chests.RemoveAll(x => has(x.UniqueId));
        _world.ScriptedEnemies.RemoveAll(x => has(x.UniqueId));
        _world.ScriptedTiles.RemoveAll(x => has(x.UniqueId));

        foreach (var item in _world.Chests)
        {
            SetTileId(item.X, item.Y, item.ClosedTileId);
        }

        foreach (var item in _world.ScriptedTiles)
        {
            SetTileId(item.X, item.Y, item.BeforeTileId);
        }
    }

    public bool CheckPortals(int posX, int posY, out WorldPortal worldPortal)
    {
        worldPortal = _world.Portals
            .Where(x => x.MatchPos(posX, posY))
            .FirstOrDefault();

        return worldPortal is not null;
    }

    public bool CheckStores(int posX, int posY, out StorePortal storePortal)
    {
        storePortal = _world.Stores
            .Where(x => x.MatchPos(posX, posY))
            .FirstOrDefault();

        return storePortal is not null;
    }

    public bool CheckMessagePoints(int posX, int posY, out MessagePoint messagePoint)
    {
        messagePoint = _world.MessagePoints
            .Where(x => x.MatchPos(posX, posY))
            .FirstOrDefault();

        return messagePoint is not null;
    }

    public bool CheckRestPoints(int posX, int posY, out RestPoint restPoint)
    {
        restPoint = _world.RestPoints
            .Where(x => x.MatchPos(posX, posY))
            .FirstOrDefault();

        return restPoint is not null;
    }

    public bool CheckChests(int posX, int posY, out ChestPoint chestPoint)
    {
        chestPoint = _world.Chests
            .Where(x => x.MatchPos(posX, posY))
            .FirstOrDefault();

        return chestPoint is not null;
    }

    public bool CheckScriptedEnemies(int posX, int posY, out ScriptedEnemy scriptedEnemy)
    {
        scriptedEnemy = _world.ScriptedEnemies
            .Where(x => x.MatchPos(posX, posY))
            .FirstOrDefault();

        return scriptedEnemy is not null;
    }

    public bool CheckScriptedTiles(int posX, int posY, out ScriptedTile scriptedTile)
    {
        scriptedTile = _world.ScriptedTiles
            .Where(x => x.MatchPos(posX, posY))
            .FirstOrDefault();

        return scriptedTile is not null;
    }

    public void RenderWorld(int posX, int posY, AvatarFacing avatarFacing)
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
        if (_world is null)
        {
            return;
        }

        _backgrounds[_world.BackgroundImage].Render();
    }

    public void RenderBackground(int backgroundId)
    {
        if (backgroundId >= 0 ||
            backgroundId < _backgrounds.Length)
        {
            _backgrounds[backgroundId].Render();
        }
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

    public void SetTileId(int posX, int posY, int tileId)
    {
        if (CheckBounds(posX, posY))
        {
            // Note: x,y flipped to ease map making
            _world.Tiles[posY, posX] = tileId;
        }
    }

    private bool CheckBounds(int posX, int posY)
    {
        if (_world is null)
        {
            Console.WriteLine("Error: Unable to check bounds for ({0};{1})", posX, posY);
            return false;
        }

        // Note: x,y flipped to ease map making
        if (posX >= 0 &&
            posX < _world.Width &&
            posY >= 0 &&
            posY < _world.Height)
        {
            return true;
        }

        return false;
    }

    private int GetTileId(int posX, int posY)
    {
        if (!CheckBounds(posX, posY))
        {
            return 0;
        }

        // Note: x,y flipped to ease map making
        return _world.Tiles[posY, posX];
    }

    private void RenderTile(int posX, int posY, int layoutId)
    {
        int tileId = GetTileId(posX, posY);

        if (tileId <= 0 ||
            tileId >= _tileSet.Length)
        {
            return;
        }

        var layout = _tileLayouts[layoutId];
        _tileSet[tileId].Image.Render(
            layout.SrcX,
            layout.SrcY,
            layout.Width,
            layout.Height,
            layout.DstX + TileSetRenderOffsetX,
            layout.DstY + TileSetRenderOffsetY);
    }

    private void LoadTiles()
    {
        _tileSet = new[]
        {
            new Tile(null, false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/dungeon_floor.png"), true),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/dungeon_wall.png"), false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/dungeon_door.png"), true),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/pillar_exterior.png"), false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/dungeon_ceiling.png"), true),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/grass.png"), true),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/pillar_interior.png"), false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/chest_interior.png"), true),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/chest_exterior.png"), true),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/medieval_house.png"), false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/medieval_door.png"), true),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/tree_evergreen.png"), false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/grave_cross.png"), false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/grave_stone.png"), false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/water.png"), false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/skull_pile.png"), false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/hay_pile.png"), true),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/locked_door.png"), false),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/death_speaker.png"), true),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/wood_floor.png"), true),
            new Tile(_resourceManager.LoadImage("Data/images/tiles/template_floor.png"), true),
        };
    }

    private void LoadBackgrounds()
    {
        _backgrounds = new[]
        {
            _resourceManager.LoadImage("Data/images/backgrounds/black.png"),
            _resourceManager.LoadImage("Data/images/backgrounds/nightsky.png"),
            _resourceManager.LoadImage("Data/images/backgrounds/tempest.png"),
            _resourceManager.LoadImage("Data/images/backgrounds/interior.png"),
        };
    }

    private void LoadMinimap()
    {
        _minimap = new Minimap(
           Width,
           Height,
           _resourceManager.LoadImage("Data/images/interface/minimap.png"),
           _resourceManager.LoadImage("Data/images/interface/minimap_cursor.png"))
        {
            X = 4,
            Y = 4,
        };

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                MinimapTileType tileType = CheckPortals(i, j, out _) ? MinimapTileType.Portal :
                    CheckStores(i, j, out _) ? MinimapTileType.Store :
                    GetTile(i, j).Walkable ? MinimapTileType.Walkable : MinimapTileType.Blocked;

                _minimap.SetTileType(i, j, tileType);
            }
        }
    }
}
