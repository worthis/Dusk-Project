namespace DuskProject.Source.Maze
{
    public record MazePointBase
    {
        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public bool CheckEnter(int posX, int posY)
        {
            return posX.Equals(X) &&
                posY.Equals(Y);
        }
    }
}
