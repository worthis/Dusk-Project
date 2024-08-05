namespace DuskProject.Source.UI
{
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;

    public class DialogButton : Button
    {
        private DialogButtonAction _action = DialogButtonAction.None;

        public DialogButton(int x, int y, int width, int height, ImageResource image, int imageCoordX = 0, int imageCoordY = 0)
            : base(x, y, width, height, image, imageCoordX, imageCoordY)
        {
        }

        public DialogButtonAction Action
        {
            get { return _action; }
            set { SetAction(value); }
        }

        public string TextFirst { get; set; } = string.Empty;

        public string TextSecond { get; set; } = string.Empty;

        public override void Render()
        {
            if (_action == DialogButtonAction.None)
            {
                return;
            }

            base.Render();
        }

        private void SetAction(DialogButtonAction action)
        {
            _action = action;
            ImageCoordX = ((int)action - 1) * Width;
            ImageCoordY = 0;
        }
    }
}
