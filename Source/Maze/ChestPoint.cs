namespace DuskProject.Source.Maze
{
    using DuskProject.Source.Enums;

    public record ChestPoint
    {
        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public string UniqueId { get; set; } = string.Empty;

        public int CloseTileId { get; set; } = 0;

        public int OpenedTileId { get; set; } = 0;

        public ChestRewardType RewardType { get; set; } = ChestRewardType.None;

        public string RewardItemId { get; set; } = string.Empty;

        public int RewardItemAmount { get; set; } = 0;

        public SoundFX Sound { get; set; } = SoundFX.Coin;

        public bool CheckEnter(int posX, int posY)
        {
            return posX == X && posY == Y;
        }
    }
}
