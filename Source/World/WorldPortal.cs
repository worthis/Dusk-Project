namespace DuskProject.Source.World
{
    public record WorldPortal : WorldPoint
    {
        public string Destination { get; set; }

        public int DestX { get; set; }

        public int DestY { get; set; }
    }
}
