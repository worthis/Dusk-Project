namespace DuskProject.Source.World;

public record WorldPortal : WorldPoint
{
    public string Destination { get; init; }

    public int DestX { get; init; }

    public int DestY { get; init; }
}
