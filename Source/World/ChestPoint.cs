namespace DuskProject.Source.World;

using DuskProject.Source.Enums;

public record ChestPoint : WorldPoint
{
    public string UniqueId { get; init; } = string.Empty;

    public int ClosedTileId { get; init; } = 0;

    public int OpenedTileId { get; init; } = 0;

    public ChestRewardType RewardType { get; init; } = ChestRewardType.None;

    public string RewardItemId { get; init; } = string.Empty;

    public int RewardItemAmount { get; init; } = 0;

    public SoundFX Sound { get; init; } = SoundFX.Coin;
}
