namespace DuskProject.Source.Maze
{
    public record StorePortal : MazePointBase
    {
        public string Store { get; set; }

        public int DestX { get; set; }

        public int DestY { get; set; }
    }
}
