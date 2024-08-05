namespace DuskProject.Source
{
    using DuskProject.Source.Resources;

    public class ResourceManager
    {
        private static ResourceManager instance;
        private static object instanceLock = new object();

        private IntPtr _screenSurfacePtr = IntPtr.Zero;
        private Dictionary<string, WeakReference> _resources = new();

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

        public void Init(IntPtr screenSurfacePtr)
        {
            if (screenSurfacePtr == IntPtr.Zero)
            {
                Console.WriteLine("Error: Unable to initialize ResourceManager");
                return;
            }

            _screenSurfacePtr = screenSurfacePtr;

            Console.WriteLine("ResourceManager initialized");
        }

        public ImageResource LoadImage(string fileName, bool alpha = false)
        {
            if (_resources.TryGetValue(fileName, out WeakReference weakReference))
            {
                if (weakReference.IsAlive)
                {
                    return weakReference.Target as ImageResource;
                }

                _resources.Remove(fileName);
            }

            ImageResource image = new ImageResource(fileName, alpha, _screenSurfacePtr);
            _resources.Add(fileName, new WeakReference(image));
            return image;
        }

        public SoundResource LoadSound(string fileName)
        {
            if (_resources.TryGetValue(fileName, out WeakReference weakReference))
            {
                if (weakReference.IsAlive)
                {
                    return weakReference.Target as SoundResource;
                }

                _resources.Remove(fileName);
            }

            SoundResource sound = new SoundResource(fileName);
            _resources.Add(fileName, new WeakReference(sound));
            return sound;
        }

        public MusicResource LoadMusic(string fileName)
        {
            if (_resources.TryGetValue(fileName, out WeakReference weakReference))
            {
                if (weakReference.IsAlive)
                {
                    return weakReference.Target as MusicResource;
                }

                _resources.Remove(fileName);
            }

            MusicResource music = new MusicResource(fileName);
            _resources.Add(fileName, new WeakReference(music));
            return music;
        }
    }
}
