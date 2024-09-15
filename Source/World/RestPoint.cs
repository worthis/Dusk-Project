namespace DuskProject.Source.World;

using DuskProject.Source.Enums;

public record RestPoint : WorldPoint
{
    public string Message { get; init; } = string.Empty;

    public SoundFX Sound { get; init; } = SoundFX.Coin;
}
