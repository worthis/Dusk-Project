namespace DuskProject.Source.World
{
    using Newtonsoft.Json;

    public class WorldData
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;

        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        [JsonProperty]
        public string Music { get; set; } = string.Empty;

        [JsonProperty]
        public int Width { get; set; } = 0;

        [JsonProperty]
        public int Height { get; set; } = 0;

        [JsonProperty]
        public int BackgroundImage { get; set; } = 0;

        [JsonProperty]
        public int[,] Tiles { get; set; } = new int[0, 0];

        [JsonProperty]
        public List<WorldPortal> Portals { get; set; } = new();

        [JsonProperty]
        public List<StorePortal> Stores { get; set; } = new();

        [JsonProperty]
        public List<string> Enemies { get; set; } = new();

        public List<RestPoint> RestPoints { get; set; } = new();

        [JsonProperty]
        public List<ChestPoint> Chests { get; set; } = new();

        [JsonProperty]
        public List<MessagePoint> MessagePoints { get; set; } = new();

        [JsonProperty]
        public List<ScriptedEnemy> ScriptedEnemies { get; set; } = new();

        [JsonProperty]
        public List<ScriptedTile> ScriptedTiles { get; set; } = new();
    }
}
