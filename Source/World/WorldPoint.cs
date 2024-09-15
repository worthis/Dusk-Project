namespace DuskProject.Source.World;

public record WorldPoint
{
    public int X { get; init; } = 0;

    public int Y { get; init; } = 0;

    public bool MatchPos(int posX, int posY)
    {
        return posX.Equals(X) &&
            posY.Equals(Y);
    }
}
