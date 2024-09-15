namespace DuskProject.Source.GameStates.Title;

using DuskProject.Source.GameStates.Title.Pages;
using DuskProject.Source.Interfaces;

public class TitleState : IGameState
{
    private GameStateManager _gameStateManager;
    private WindowManager _windowManager;
    private ResourceManager _resourceManager;
    private SoundManager _soundManager;
    private TextManager _textManager;

    public TitleState(
        GameStateManager gameStateManager,
        WindowManager windowManager,
        ResourceManager resourceManager,
        SoundManager soundManager,
        TextManager textManager)
    {
        _gameStateManager = gameStateManager;
        _windowManager = windowManager;
        _resourceManager = resourceManager;
        _soundManager = soundManager;
        _textManager = textManager;

        Page = new TitleMainPage(this);

        Console.WriteLine("Title State created");
    }

    public GameStateManager GameStateManager { get => _gameStateManager; }

    public WindowManager WindowManager { get => _windowManager; }

    public ResourceManager ResourceManager { get => _resourceManager; }

    public SoundManager SoundManager { get => _soundManager; }

    public TextManager TextManager { get => _textManager; }

    public TitlePageBase Page { get; set; }

    public void Render()
    {
        Page.Render();
    }

    public void Update()
    {
        Page.Update();
    }
}
