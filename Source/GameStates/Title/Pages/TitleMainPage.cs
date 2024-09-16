namespace DuskProject.Source.GameStates.Title.Pages;

using DuskProject.Source.Enums;
using DuskProject.Source.Resources;

public class TitleMainPage : TitlePageBase
{
    private ImageResource _imageBackground;
    private int _menuSelected = 0;
    private int _menuCount = 3;

    public TitleMainPage(TitleState titleState)
        : base(titleState)
    {
        _imageBackground = Title.ResourceManager.LoadImage("Data/images/backgrounds/title.png");
    }

    public override void Render()
    {
        _imageBackground.Render();

        RenderMenuItem("START", 0, 100);
        RenderMenuItem("OPTIONS", 1, 116);
        RenderMenuItem("QUIT", 2, 132);

        Title.TextManager.Render("by Worthis, 2024", 160, 200, TextJustify.JUSTIFY_CENTER);
        Title.TextManager.Render("ft. music by Yubatake", 160, 200 + 16, TextJustify.JUSTIFY_CENTER);
    }

    public override void Update()
    {
        HandleMenuNavigation();

        if (Title.WindowManager.KeyPressed(InputKey.KEY_A))
        {
            Title.SoundManager.PlaySound(SoundFX.Click);
            ExecuteMenuAction(_menuSelected);
        }
    }

    private static string GetMenuItemText(string text, int position, int selected)
    {
        return selected.Equals(position) ? $"[ {text} ]" : text;
    }

    private void RenderMenuItem(string text, int position, int yOffset)
    {
        Title.TextManager.Render(
            GetMenuItemText(text, position, _menuSelected),
            160,
            yOffset,
            TextJustify.JUSTIFY_CENTER);
    }

    private void HandleMenuNavigation()
    {
        if (Title.WindowManager.KeyPressed(InputKey.KEY_UP))
        {
            _menuSelected = Math.Max(_menuSelected - 1, 0);
        }

        if (Title.WindowManager.KeyPressed(InputKey.KEY_DOWN))
        {
            _menuSelected = Math.Min(_menuSelected + 1, _menuCount - 1);
        }
    }

    private void ExecuteMenuAction(int selected)
    {
        switch (selected)
        {
            case 0:
                Title.GameStateManager.StartGame();
                break;
            case 1:
                // Options logic
                break;
            case 2:
                Title.GameStateManager.RegisterQuit();
                break;
        }
    }
}
