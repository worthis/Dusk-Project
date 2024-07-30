namespace DuskProject.Source
{
    using DuskProject.Source.Resources;

    public class ResourceManager
    {
        private static ResourceManager instance;
        private static object instanceLock = new object();

        private Dictionary<string, ImageResource> _imageResources = new Dictionary<string, ImageResource>();

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

        public ImageResource LoadImage(string fileName)
        {
            ImageResource image;

            if (_imageResources.TryGetValue(fileName, out image))
            {
                image.Load();
                return image;
            }

            image = new ImageResource(fileName, false);
            image.Load();

            _imageResources.Add(fileName, image);

            return image;
        }

        public void Free(CommonResource resource)
        {
        }
    }
}
