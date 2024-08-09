namespace DuskProject.Source.Maze
{
    using DuskProject.Source.Enums;

    public record MessagePoint : MazePointBase
    {
        public string UniqueId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public SoundFX Sound { get; set; } = SoundFX.None;
    }
}
