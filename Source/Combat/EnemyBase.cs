namespace DuskProject.Source.Combat
{
    using DuskProject.Source.Enums;
    using Newtonsoft.Json;

    public class EnemyBase
    {
        [JsonIgnore]
        public string Id { get; protected set; } = string.Empty;

        [JsonProperty]
        public string Name { get; protected set; } = string.Empty;

        [JsonProperty]
        public string ImageFile { get; protected set; } = string.Empty;

        [JsonProperty]
        public EnemyCategory Category { get; protected set; }

        [JsonProperty]
        public int HP { get; protected set; }

        [JsonProperty]
        public int AttackMin { get; protected set; }

        [JsonProperty]
        public int AttackMax { get; protected set; }

        [JsonProperty]
        public int GoldMin { get; protected set; }

        [JsonProperty]
        public int GoldMax { get; protected set; }

        [JsonProperty]
        public List<CombatPowers> Powers { get; protected set; } = new List<CombatPowers>();
    }
}
