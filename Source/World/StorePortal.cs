namespace DuskProject.Source.World
{
    public record StorePortal : WorldPoint
    {
        public string Store { get; set; }

        public int DestX { get; set; }

        public int DestY { get; set; }
    }
}
