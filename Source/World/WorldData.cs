namespace DuskProject.Source.World;

using Newtonsoft.Json;

public class WorldData
{
    [JsonIgnore]
    public string Id { get; set; } = string.Empty;

    [JsonProperty]
    public string Name { get; init; } = string.Empty;

    [JsonProperty]
    public string Music { get; init; } = string.Empty;

    [JsonProperty]
    public int Width { get; init; } = 0;

    [JsonProperty]
    public int Height { get; init; } = 0;

    [JsonProperty]
    public int BackgroundImage { get; init; } = 0;

    [JsonProperty]
    public int[,] Tiles { get; init; } = new int[0, 0];

    [JsonProperty]
    public List<WorldPortal> Portals { get; init; } = new();

    [JsonProperty]
    public List<StorePortal> Stores { get; init; } = new();

    [JsonProperty]
    public List<string> Enemies { get; init; } = new();

    public List<RestPoint> RestPoints { get; init; } = new();

    [JsonProperty]
    public List<ChestPoint> Chests { get; init; } = new();

    [JsonProperty]
    public List<MessagePoint> MessagePoints { get; init; } = new();

    [JsonProperty]
    public List<ScriptedEnemy> ScriptedEnemies { get; init; } = new();

    [JsonProperty]
    public List<ScriptedTile> ScriptedTiles { get; init; } = new();
}
