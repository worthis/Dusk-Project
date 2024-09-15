namespace DuskProject.Source.World;

public record TileLayout
{
    public int Width { get; init; }

    public int Height { get; init; }

    public int SrcX { get; init; }

    public int SrcY { get; init; }

    public int DstX { get; init; }

    public int DstY { get; init; }
}
