namespace DuskProject.Source.MainMenu
{
    public class MainMenuManager
    {
        private IMenuState _state;

        public MainMenuManager()
        {
            _state = new MenuTitleState(this);
        }

        public void Update()
        {
            _state.Update();
        }

        public void Render()
        {
            _state.Render();
        }
    }
}
