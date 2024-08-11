namespace DuskProject.Source.World
{
    using DuskProject.Source.Resources;

    public record Tile
    {
        private ImageResource _image;
        private bool _walkable;

        public Tile(ImageResource image, bool walkable)
        {
            _image = image;
            _walkable = walkable;
        }

        public ImageResource Image { get => _image; }

        public bool Walkable { get => _walkable; }

        public string Name
        {
            get
            {
                if (_image is not null)
                {
                    return _image.Name;
                }

                return string.Empty;
            }
        }
    }
}
