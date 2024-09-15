namespace DuskProject.Source.Dialog;

using DuskProject.Source.Enums;
using Newtonsoft.Json;

public record Item
{
    [JsonIgnore]
    public string Id { get; init; }

    [JsonProperty]
    public string Name { get; init; } = string.Empty;

    [JsonProperty]
    public ItemType Type { get; init; }

    [JsonProperty]
    public int Gold { get; init; } = 0;

    [JsonProperty]
    public int AttackMin { get; init; } = 0;

    [JsonProperty]
    public int AttackMax { get; init; } = 0;

    [JsonProperty]
    public int Defence { get; init; } = 0;

    [JsonProperty]
    public int Level { get; init; } = 0;

    public int AttackAvg()
    {
        return (int)((AttackMin + AttackMax) * 0.5);
    }

    public int AttackDispersion()
    {
        return AttackMax - AttackMin;
    }
}
