namespace DuskProject.Source.World;

public record ScriptedTile : WorldPoint
{
    public string UniqueId { get; init; } = string.Empty;

    public string RequiredAction { get; init; } = string.Empty;

    public int BeforeTileId { get; init; } = 0;

    public int AfterTileId { get; init; } = 0;
}
