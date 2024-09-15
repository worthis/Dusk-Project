namespace DuskProject.Source.World;

public record StorePortal : WorldPoint
{
    public string Store { get; init; }

    public int DestX { get; init; }

    public int DestY { get; init; }
}
