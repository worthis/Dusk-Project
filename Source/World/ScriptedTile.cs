namespace DuskProject.Source.World
{
    public record ScriptedTile : WorldPoint
    {
        public string UniqueId { get; set; } = string.Empty;

        public string RequiredAction { get; set; } = string.Empty;

        public int BeforeTileId { get; set; } = 0;

        public int AfterTileId { get; set; } = 0;
    }
}
