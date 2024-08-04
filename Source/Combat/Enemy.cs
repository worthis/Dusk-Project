namespace DuskProject.Source.Combat
{
    using DuskProject.Source.Resources;

    public class Enemy : EnemyBase
    {
        public int RenderOffsetX { get; set; } = 0;

        public int RenderOffsetY { get; set; } = 0;

        public ImageResource Image { get; set; }

        public int AttackDispersion()
        {
            return AttackMax - AttackMin;
        }

        public int GoldDispersion()
        {
            return GoldMax - GoldMin;
        }

        public void Render()
        {
            Image.Render(
                0,
                0,
                Image.Width,
                Image.Height,
                RenderOffsetX,
                RenderOffsetY);
        }
    }
}
