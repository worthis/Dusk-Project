namespace DuskProject.Source.Resources
{
    using System;
    using System.Runtime.InteropServices;
    using SDL2;

    public class ImageResource : CommonResource
    {
        private IntPtr _image;
        private SDL.SDL_Surface _imageSurface;

        public ImageResource(string filename, bool alpha)
        {
            Name = filename;

            _image = SDL_image.IMG_Load(Name);

            if (_image == IntPtr.Zero)
            {
                Console.WriteLine("Error: Failed to load image {0}", Name);
                return;
            }

            _imageSurface = Marshal.PtrToStructure<SDL.SDL_Surface>(_image);

            if (_imageSurface.w > 1920 || _imageSurface.h > 1080)
            {
                Console.WriteLine("Error: Failed to load image {0}, dimensions more than 1920x1080", Name);
                SDL.SDL_FreeSurface(_image);

                return;
            }

            /*if (alpha)
            {
                SDL.SDL_SetColorKey(_image, (int)SDL.SDL_bool.SDL_TRUE, SDL.SDL_MapRGB(_imageSurface.format, 0, 0, 255));
            }*/

            Console.WriteLine("Loaded image {0} [{1}x{2}]", Name, Width, Height);
        }

        public int Width
        {
            get { return _imageSurface.w; }
        }

        public int Height
        {
            get { return _imageSurface.h; }
        }

        public IntPtr GetImage()
        {
            return _image;
        }
    }
}
