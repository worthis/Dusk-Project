namespace DuskProject.Source.Resources
{
    using System;
    using SDL2;

    public class SoundResource : CommonResource, IDisposable
    {
        private IntPtr _sound;
        private bool _disposedValue;

        public SoundResource(string fileName)
        {
            SetName(fileName);
            _sound = SDL_mixer.Mix_LoadWAV(fileName);
            Console.WriteLine("Loaded sound {0}", fileName);
        }

        ~SoundResource()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IntPtr GetSound()
        {
            return _sound;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (_sound != IntPtr.Zero)
                {
                    SDL_mixer.Mix_FreeChunk(_sound);
                    _sound = IntPtr.Zero;
                }

                _disposedValue = true;
            }
        }
    }
}
