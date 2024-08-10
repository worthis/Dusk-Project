namespace DuskProject.Source
{
    using DuskProject.Source.Dialog;
    using DuskProject.Source.Enums;
    using Newtonsoft.Json;

    public class AvatarBase
    {
        [JsonProperty]
        public int HP { get; protected set; } = 25;

        [JsonProperty]
        public int MaxHP { get; set; } = 25;

        [JsonProperty]
        public int MP { get; protected set; } = 4;

        [JsonProperty]
        public int MaxMP { get; set; } = 4;

        public int PosX { get; set; } = 1;

        public int PosY { get; set; } = 1;

        [JsonIgnore]
        public bool Moved { get; protected set; } = false;

        [JsonProperty]
        public AvatarFacing Facing { get; protected set; } = AvatarFacing.South;

        public string MazeWorld { get; set; } = "0-serf-quarters";

        [JsonProperty]
        public string SleepMazeWorld { get; protected set; } = "0-serf-quarters";

        [JsonProperty]
        public int SleepPosX { get; protected set; } = 1;

        [JsonProperty]
        public int SleepPosY { get; protected set; } = 1;

        [JsonProperty]
        public int Gold { get; protected set; } = 0;

        [JsonProperty]
        public Item Weapon { get; protected set; }

        [JsonProperty]
        public int Attack { get; set; } = 0;

        [JsonProperty]
        public Item Armor { get; protected set; }

        [JsonProperty]
        public int Defence { get; set; } = 0;

        [JsonProperty]
        public int SpellBookLevel { get; protected set; } = 0;

        [JsonProperty]
        public List<Item> SpellBook { get; protected set; } = new List<Item>();

        [JsonProperty]
        public List<string> Campaign { get; protected set; } = new List<string>();
    }
}
