namespace DuskProject.Source.Dialog
{
    using DuskProject.Source.Enums;
    using Newtonsoft.Json;

    public class Item
    {
        [JsonIgnore]
        public string Id { get; protected set; }

        [JsonProperty]
        public string Name { get; protected set; } = string.Empty;

        [JsonProperty]
        public ItemType Type { get; protected set; }

        [JsonProperty]
        public int Gold { get; protected set; } = 0;

        [JsonProperty]
        public int AttackMin { get; protected set; } = 0;

        [JsonProperty]
        public int AttackMax { get; protected set; } = 0;

        [JsonProperty]
        public int Defence { get; protected set; } = 0;

        [JsonProperty]
        public int Level { get; protected set; } = 0;

        public int AttackAvg()
        {
            return (int)((AttackMin + AttackMax) * 0.5);
        }
    }
}
