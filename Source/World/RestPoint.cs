namespace DuskProject.Source.World
{
    using DuskProject.Source.Enums;

    public record RestPoint : WorldPoint
    {
        public string Message { get; set; } = string.Empty;

        public SoundFX Sound { get; set; } = SoundFX.Coin;
    }
}
