namespace DuskProject.Source.UI
{
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;

    public class InfoButton : Button
    {
        private InfoButtonType _spell;

        public InfoButton(InfoButtonType spellType, int x, int y, int width, int height, ImageResource image, int imageCoordX = 0, int imageCoordY = 0)
            : base(x, y, width, height, image, imageCoordX, imageCoordY)
        {
            Spell = spellType;
        }

        public InfoButtonType Spell
        {
            get { return _spell; }
            set { SetSpell(value); }
        }

        private void SetSpell(InfoButtonType spell)
        {
            _spell = spell;
            ImageCoordX = ((int)spell - 1) * Width;
            ImageCoordY = 0;
        }
    }
}
