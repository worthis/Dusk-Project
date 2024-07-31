namespace DuskProject.Source.Resources
{
    public class CommonResource
    {
        private int _count;
        private string _name;

        public int Count
        {
            get { return _count; }
        }

        public string Name { get => _name; }

        public void Load()
        {
            _count++;
        }

        public void Unload()
        {
            _count--;
        }

        protected void SetName(string name)
        {
            _name = name;
        }
    }
}
