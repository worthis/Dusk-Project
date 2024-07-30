namespace DuskProject.Source.MazeObjects
{
    public class MazeWorld
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Music { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int BackgroundImage { get; set; }

        public int[,] Tiles { get; set; }

        public List<MazePortal> Portals { get; set; }

        public List<ShopPortal> Shops { get; set; }

        public List<string> Enemies { get; set; }
    }
}
