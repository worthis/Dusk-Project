namespace DuskProject.Source.UI
{
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;

    public class InfoButton : Button
    {
        private ActionType _action;

        public InfoButton(ActionType action, int x, int y, int width, int height, ImageResource image, int imageCoordX = 0, int imageCoordY = 0)
            : base(x, y, width, height, image, imageCoordX, imageCoordY)
        {
            Action = action;
        }

        public ActionType Action
        {
            get { return _action; }
            set { SetAction(value); }
        }

        private void SetAction(ActionType action)
        {
            _action = action;
            ImageCoordX = ((int)action) * Width;
            ImageCoordY = 0;
        }
    }
}
