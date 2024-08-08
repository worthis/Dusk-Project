namespace DuskProject.Source.Maze
{
    public record StorePortal
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string Store { get; set; }

        public int DestX { get; set; }

        public int DestY { get; set; }

        public bool CheckEnter(int posX, int posY)
        {
            return posX == X && posY == Y;
        }
    }
}
