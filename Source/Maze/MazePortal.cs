namespace DuskProject.Source.Maze
{
    public record MazePortal : MazePointBase
    {
        public string Destination { get; set; }

        public int DestX { get; set; }

        public int DestY { get; set; }
    }
}
