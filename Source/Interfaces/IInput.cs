namespace DuskProject.Source.Interfaces;

using DuskProject.Source.Enums;

public interface IInput
{
    public bool KeyPressed(InputKey key);

    public void ProcessInput();
}
