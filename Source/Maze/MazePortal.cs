namespace DuskProject.Source.Maze
{
    public class MazePortal
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string Destination { get; set; }

        public int DestX { get; set; }

        public int DestY { get; set; }

        public bool CheckEnter(int posX, int posY)
        {
            return posX == X && posY == Y;
        }
    }
}
