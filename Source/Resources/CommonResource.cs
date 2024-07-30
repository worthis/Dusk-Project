namespace DuskProject.Source.Resources
{
    public class CommonResource
    {
        private string _name;
        private int _count;

        public int Count
        {
            get { return _count; }
        }

        protected string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void Load()
        {
            _count++;
        }

        public void Unload()
        {
            _count--;
        }
    }
}
