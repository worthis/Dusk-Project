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

        Title.TextManager.Render(GetMenuItemText("START", 0, _menuSelected), 160, 100, TextJustify.JUSTIFY_CENTER);
        Title.TextManager.Render(GetMenuItemText("OPTIONS", 1, _menuSelected), 160, 100 + 16, TextJustify.JUSTIFY_CENTER);
        Title.TextManager.Render(GetMenuItemText("QUIT", 2, _menuSelected), 160, 100 + 16 + 16, TextJustify.JUSTIFY_CENTER);

        Title.TextManager.Render("by Worthis, 2024", 160, 200, TextJustify.JUSTIFY_CENTER);
        Title.TextManager.Render("ft. music by Yubatake", 160, 200 + 16, TextJustify.JUSTIFY_CENTER);
    }

    public override void Update()
    {
        if (Title.WindowManager.KeyPressed(InputKey.KEY_UP))
        {
            _menuSelected--;
            if (_menuSelected < 0)
            {
                _menuSelected = 0;
            }
        }

        if (Title.WindowManager.KeyPressed(InputKey.KEY_DOWN))
        {
            _menuSelected++;
            if (_menuSelected >= _menuCount)
            {
                _menuSelected = _menuCount - 1;
            }
        }

        if (Title.WindowManager.KeyPressed(InputKey.KEY_A))
        {
            switch (_menuSelected)
            {
                // Start
                case 0:
                    Title.SoundManager.PlaySound(SoundFX.Click);
                    Title.GameStateManager.StartGame();
                    break;

                // Options
                case 1:
                    Title.SoundManager.PlaySound(SoundFX.Click);
                    break;

                // Quit
                case 2:
                    Title.SoundManager.PlaySound(SoundFX.Click);
                    Title.GameStateManager.RegisterQuit();
                    break;
            }
        }
    }

    private static string GetMenuItemText(string menuItemText, int menuItemPos, int menuSelected)
    {
        return menuSelected.Equals(menuItemPos) ? string.Format("[ {0} ]", menuItemText) : menuItemText;
    }
}
