namespace DuskProject.Source.Dialog
{
    using DuskProject.Source.Enums;

    public class DialogLine
    {
        public DialogType Type { get; set; }

        public string Value { get; set; }

        public int Cost { get; set; }

        public string MessageFirst { get; set; }

        public string MessageSecond { get; set; }
    }
}
