namespace DuskProject.Source
{
    using DuskProject.Source.Resources;

    public class ResourceManager
    {
        private static ResourceManager instance;
        private static object instanceLock = new object();

        private Dictionary<string, ImageResource> _imageResources = new Dictionary<string, ImageResource>();
        private Dictionary<string, SoundResource> _soundResources = new Dictionary<string, SoundResource>();
        private Dictionary<string, MusicResource> _musicResources = new Dictionary<string, MusicResource>();

        private ResourceManager()
        {
        }

        public static ResourceManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ResourceManager();
                        Console.WriteLine("ResourceManager created");
                    }
                }
            }

            return instance;
        }

        public ImageResource LoadImage(string fileName, bool alpha = false)
        {
            ImageResource image;

            if (_imageResources.TryGetValue(fileName, out image))
            {
                image.Load();
                return image;
            }

            image = new ImageResource(fileName, alpha);
            image.Load();

            _imageResources.Add(fileName, image);

            return image;
        }

        public SoundResource LoadSound(string fileName)
        {
            SoundResource sound;

            if (_soundResources.TryGetValue(fileName, out sound))
            {
                sound.Load();
                return sound;
            }

            sound = new SoundResource(fileName);
            sound.Load();

            _soundResources.Add(fileName, sound);

            return sound;
        }

        public MusicResource LoadMusic(string fileName)
        {
            MusicResource music;

            if (_musicResources.TryGetValue(fileName, out music))
            {
                music.Load();
                return music;
            }

            music = new MusicResource(fileName);
            music.Load();

            _musicResources.Add(fileName, music);

            return music;
        }

        public void Free(CommonResource resource)
        {
            if (resource is null)
            {
                return;
            }

            resource.Unload();

            if (resource.Count <= 0)
            {
                string name = resource.Name;

                if (resource is ImageResource)
                {
                    (resource as ImageResource).Dispose();
                    _imageResources.Remove(name);
                }

                if (resource is SoundResource)
                {
                    (resource as SoundResource).Dispose();
                    _soundResources.Remove(name);
                }

                if (resource is MusicResource)
                {
                    (resource as MusicResource).Dispose();
                    _musicResources.Remove(name);
                }
            }
        }

        public void Quit()
        {
            foreach (var image in _imageResources.Values)
            {
                image.Dispose();
            }

            foreach (var sound in _soundResources.Values)
            {
                sound.Dispose();
            }

            foreach (var music in _musicResources.Values)
            {
                music.Dispose();
            }

            _imageResources.Clear();
            _soundResources.Clear();
            _musicResources.Clear();
        }
    }
}
