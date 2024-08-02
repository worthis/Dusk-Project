namespace DuskProject.Source.Dialog
{
    using DuskProject.Source.Enums;

    public class Button
    {
        public Button(int x, int y, int width, int height)
        {
            Position = new ButtonPosition(x, y, width, height);
        }

        public DialogButtonAction Action { get; set; } = DialogButtonAction.None;

        public ButtonPosition Position { get; set; }

        public bool Selected { get; set; } = false;

        public bool Enabled { get; set; } = false;

        public string TextFirst { get; set; } = string.Empty;

        public string TextSecond { get; set; } = string.Empty;
    }
}
