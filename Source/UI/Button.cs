namespace DuskProject.Source.UI
{
    using DuskProject.Source.Resources;

    public class Button
    {
        private ImageResource _image;
        private ImageResource _selectedImage;

        public Button(
            int x,
            int y,
            int width,
            int height,
            ImageResource image,
            int imageCoordX = 0,
            int imageCoordY = 0)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            _image = image;
            ImageCoordX = imageCoordX;
            ImageCoordY = imageCoordY;
        }

        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public int Width { get; set; }

        public int Height { get; set; }

        public bool Selected { get; set; } = false;

        public bool Enabled { get; set; } = false;

        protected int ImageCoordX { get; set; } = 0;

        protected int ImageCoordY { get; set; } = 0;

        protected int SelectedImageCoordX { get; set; } = 0;

        protected int SelectedImageCoordY { get; set; } = 0;

        protected int SelectedImageOffsetX { get; set; } = 1;

        protected int SelectedImageOffsetY { get; set; } = 1;

        protected int SelectedImageWidth { get; set; }

        protected int SelectedImageHeight { get; set; }

        public void SetSelectedImage(
            ImageResource image,
            int width,
            int height,
            int offsetX,
            int offsetY,
            int imageCoordX = 0,
            int imageCoordY = 0)
        {
            if (image is null)
            {
                return;
            }

            _selectedImage = image;
            SelectedImageCoordX = imageCoordX;
            SelectedImageCoordY = imageCoordY;
            SelectedImageOffsetX = offsetX;
            SelectedImageOffsetY = offsetY;
            SelectedImageWidth = width;
            SelectedImageHeight = height;
        }

        public virtual void Render()
        {
            if (!Enabled)
            {
                return;
            }

            if (Selected)
            {
                RenderSelected();
            }

            _image.Render(
                ImageCoordX,
                ImageCoordY,
                Width,
                Height,
                X,
                Y);
        }

        protected virtual void RenderSelected()
        {
            if (_selectedImage is null)
            {
                return;
            }

            _selectedImage.Render(
                SelectedImageCoordX,
                SelectedImageCoordY,
                SelectedImageWidth,
                SelectedImageHeight,
                X - SelectedImageOffsetX,
                Y - SelectedImageOffsetY);
        }
    }
}
