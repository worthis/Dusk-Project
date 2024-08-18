namespace DuskProject.Source.Input
{
    using DuskProject.Source.Enums;

    public interface IInput
    {
        public bool KeyPressed(InputKey key);

        public void ProcessInput();
    }
}
