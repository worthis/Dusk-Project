namespace DuskProject.Source.Combat
{
    using DuskProject.Source.Dialog;
    using DuskProject.Source.Enums;

    public class InfoButton : Button
    {
        public InfoButton(InfoButtonType spellType, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Spell = spellType;
        }

        public InfoButtonType Spell { get; set; } = InfoButtonType.None;
    }
}
