namespace DuskProject.Source.Input
{
    using DuskProject.Source.Enums;
    using DuskProject.Source.Interfaces;

    public abstract class InputBase : IInput
    {
        protected List<InputKey> KeyPressedList { get; set; } = new List<InputKey>();

        public bool KeyPressed(InputKey key)
        {
            return KeyPressedList.Contains(key);
        }

        public abstract void ProcessInput();
    }
}
