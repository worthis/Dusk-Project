﻿namespace DuskProject.Source.Combat
{
    using DuskProject.Source.Resources;

    public class Enemy : EnemyBase
    {
        public int RenderOffsetX { get; set; } = 0;

        public int RenderOffsetY { get; set; } = 0;

        public ImageResource Image { get; set; }
    }
}
