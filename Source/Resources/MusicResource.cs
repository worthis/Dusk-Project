namespace DuskProject.Source.Resources
{
    using System;
    using SDL2;

    public class MusicResource : CommonResource, IDisposable
    {
        private IntPtr _music;
        private bool _disposedValue;

        public MusicResource(string fileName)
        {
            Name = fileName;
            _music = SDL_mixer.Mix_LoadMUS(fileName);
            Console.WriteLine("Loaded music {0}", fileName);
        }

        ~MusicResource()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IntPtr GetMusic()
        {
            return _music;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (_music != IntPtr.Zero)
                {
                    SDL_mixer.Mix_FreeMusic(_music);
                    _music = IntPtr.Zero;
                }

                _disposedValue = true;
            }
        }
    }
}
