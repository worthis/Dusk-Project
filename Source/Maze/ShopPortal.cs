namespace DuskProject.Source.Maze
{
    public class ShopPortal
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string ShopId { get; set; }

        public int DestX { get; set; }

        public int DestY { get; set; }

        public bool CheckEnter(int posX, int posY)
        {
            return posX == X && posY == Y;
        }
    }
}
