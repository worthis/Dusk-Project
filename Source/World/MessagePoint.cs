namespace DuskProject.Source.World;

using DuskProject.Source.Enums;

public record MessagePoint : WorldPoint
{
    public string UniqueId { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public SoundFX Sound { get; init; } = SoundFX.None;
}
