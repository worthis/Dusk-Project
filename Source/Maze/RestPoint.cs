namespace DuskProject.Source.Maze
{
    using DuskProject.Source.Enums;

    public record RestPoint
    {
        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public string Message { get; set; } = string.Empty;

        public SoundFX Sound { get; set; } = SoundFX.Coin;

        public bool CheckEnter(int posX, int posY)
        {
            return posX == X && posY == Y;
        }
    }
}
