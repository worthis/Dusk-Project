namespace DuskProject.Source.GameStates.Dialog
{
    public record Store
    {
        public string Name { get; set; }

        public string Music { get; set; }

        public int BackgroundImage { get; set; }

        public DialogLine[] Lines { get; set; }
    }
}
