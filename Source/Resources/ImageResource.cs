﻿namespace DuskProject.Source.Resources
{
    using System;
    using System.Runtime.InteropServices;
    using SDL2;

    public class ImageResource : CommonResource, IDisposable
    {
        private IntPtr _image;
        private SDL.SDL_Surface _imageSurface;
        private bool _disposedValue;

        public ImageResource(string filename, bool alpha)
        {
            SetName(filename);

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

            if (alpha)
            {
                SDL.SDL_SetColorKey(
                    _image,
                    (int)SDL.SDL_bool.SDL_TRUE,
                    SDL.SDL_MapRGB(_imageSurface.format, 0, 0, 255));
            }

            Console.WriteLine("Loaded image {0} [{1}x{2}]", Name, Width, Height);
        }

        ~ImageResource()
        {
            Dispose(disposing: false);
        }

        public int Width
        {
            get { return _imageSurface.w; }
        }

        public int Height
        {
            get { return _imageSurface.h; }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IntPtr GetImage()
        {
            return _image;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (_image != IntPtr.Zero)
                {
                    SDL.SDL_FreeSurface(_image);
                    _image = IntPtr.Zero;
                }

                _disposedValue = true;
            }
        }
    }
}
