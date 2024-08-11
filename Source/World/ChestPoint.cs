namespace DuskProject.Source.World
{
    using DuskProject.Source.Enums;

    public record ChestPoint : WorldPoint
    {
        public string UniqueId { get; set; } = string.Empty;

        public int ClosedTileId { get; set; } = 0;

        public int OpenedTileId { get; set; } = 0;

        public ChestRewardType RewardType { get; set; } = ChestRewardType.None;

        public string RewardItemId { get; set; } = string.Empty;

        public int RewardItemAmount { get; set; } = 0;

        public SoundFX Sound { get; set; } = SoundFX.Coin;
    }
}
