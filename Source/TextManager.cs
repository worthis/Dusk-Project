namespace DuskProject.Source
{
    using System;
    using System.Text.Json;
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;
    using DuskProject.Source.Text;

    public class TextManager
    {
        private static TextManager instance;
        private static object instanceLock = new object();

        private WindowManager _windowManager;
        private ResourceManager _resourceManager;

        private ImageResource _imageFont;

        private int _space = 3 * 2;
        private int _kerning = -1;

        private Dictionary<char, TextGlyph> _glyphDictionary;

        private TextManager()
        {
        }

        public static TextManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new TextManager();
                        Console.WriteLine("TextManager created");
                    }
                }
            }

            return instance;
        }

        public void Init()
        {
            _windowManager = WindowManager.GetInstance();
            _resourceManager = ResourceManager.GetInstance();

            _imageFont = _resourceManager.LoadImage("Data/images/interface/boxy_bold.png");

            LoadFontCharset("Data/font_320.json");

            Console.WriteLine("TextManager initialized");
        }

        public void Render(string text, int posX, int posY, TextJustify textJustify = TextJustify.JUSTIFY_LEFT)
        {
            int currentPosX;
            string textUpperCase = text.ToUpper();

            switch (textJustify)
            {
                case TextJustify.JUSTIFY_RIGHT:
                    currentPosX = posX - GetTextWidth(textUpperCase);
                    break;

                case TextJustify.JUSTIFY_CENTER:
                    currentPosX = posX - (int)(GetTextWidth(textUpperCase) * 0.5);
                    break;

                case TextJustify.JUSTIFY_LEFT:
                default:
                    currentPosX = posX;
                    break;
            }

            for (int i = 0; i < textUpperCase.Length; i++)
            {
                if (textUpperCase[i] == ' ')
                {
                    currentPosX += _space;
                    continue;
                }

                if (_glyphDictionary.TryGetValue(textUpperCase[i], out TextGlyph glyph))
                {
                    _windowManager.Draw(
                        _imageFont,
                        glyph.SrcX,
                        glyph.SrcY,
                        glyph.Width,
                        glyph.Height,
                        currentPosX,
                        posY);

                    currentPosX += glyph.Width + _kerning;
                }
            }
        }

        public int GetTextWidth(string text)
        {
            int textWidth = 0;
            string textUpperCase = text.ToUpper();

            for (int i = 0; i < textUpperCase.Length; i++)
            {
                if (textUpperCase[i] == ' ')
                {
                    textWidth += _space;
                    continue;
                }

                if (_glyphDictionary.TryGetValue(textUpperCase[i], out TextGlyph glyph))
                {
                    textWidth += glyph.Width + _kerning;
                }
            }

            return textWidth - _kerning;
        }

        private void LoadFontCharset(string fileName)
        {
            using (StreamReader streamReader = new(fileName))
            {
                string jsonData = streamReader.ReadToEnd();
                streamReader.Close();

                _glyphDictionary = JsonSerializer.Deserialize<Dictionary<char, TextGlyph>>(jsonData);
            }

            Console.WriteLine("Font charset {0} loaded", fileName);
        }
    }
}
