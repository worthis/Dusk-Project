namespace DuskProject.Source.World
{
    using DuskProject.Source.Enums;

    public record MessagePoint : WorldPoint
    {
        public string UniqueId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public SoundFX Sound { get; set; } = SoundFX.None;
    }
}
