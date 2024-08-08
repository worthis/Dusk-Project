namespace DuskProject.Source.Maze
{
    using DuskProject.Source.Enums;

    public record MessagePoint
    {
        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public string UniqueId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public SoundFX Sound { get; set; } = SoundFX.None;

        public bool CheckEnter(int posX, int posY)
        {
            return posX == X && posY == Y;
        }
    }
}
