namespace DuskProject.Source.Maze
{
    public record ScriptedTile
    {
        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public string UniqueId { get; set; } = string.Empty;

        public string RequiredAction { get; set; } = string.Empty;

        public int BeforeTileId { get; set; } = 0;

        public int AfterTileId { get; set; } = 0;

        public bool CheckEnter(int posX, int posY)
        {
            return posX == X && posY == Y;
        }
    }
}
