namespace DuskProject.Source.GameStates.Title.Pages;

public abstract class TitlePageBase
{
    private readonly TitleState _title;

    public TitlePageBase(TitleState titleState)
    {
        _title = titleState;
    }

    protected TitleState Title { get => _title; }

    public abstract void Render();

    public abstract void Update();
}
