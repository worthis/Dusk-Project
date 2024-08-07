namespace DuskProject.Source.Maze
{
    using Newtonsoft.Json;

    public class MazeWorld
    {
        [JsonIgnore]
        public string Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Music { get; set; }

        [JsonProperty]
        public int Width { get; set; }

        [JsonProperty]
        public int Height { get; set; }

        [JsonProperty]
        public int BackgroundImage { get; set; }

        [JsonProperty]
        public int[,] Tiles { get; set; }

        [JsonProperty]
        public List<MazePortal> Portals { get; set; }

        [JsonProperty]
        public List<StorePortal> Stores { get; set; }

        [JsonProperty]
        public List<string> Enemies { get; set; }
    }
}
