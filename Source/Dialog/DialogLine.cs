namespace DuskProject.Source.Dialog
{
    using DuskProject.Source.Enums;

    public record DialogLine
    {
        public DialogType Type { get; set; }

        public string Value { get; set; }

        public int Cost { get; set; }

        public string MessageFirst { get; set; }

        public string MessageSecond { get; set; }
    }
}
