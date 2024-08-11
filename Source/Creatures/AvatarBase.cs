namespace DuskProject.Source.Creatures
{
    using DuskProject.Source.Dialog;
    using DuskProject.Source.Enums;
    using Newtonsoft.Json;

    public class AvatarBase : CreatureBase
    {
        [JsonIgnore]
        public bool Moved { get; set; } = false;

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
