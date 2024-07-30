namespace DuskProject.Source.MazeObjects
{
    using DuskProject.Source.Resources;

    public class Tile
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
    }
}
