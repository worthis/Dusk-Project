namespace DuskProject.Source.Maze
{
    using DuskProject.Source.Enums;

    public record RestPoint : MazePointBase
    {
        public string Message { get; set; } = string.Empty;

        public SoundFX Sound { get; set; } = SoundFX.Coin;
    }
}
