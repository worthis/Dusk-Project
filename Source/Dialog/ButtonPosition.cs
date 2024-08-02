﻿namespace DuskProject.Source.Dialog
{
    public class ButtonPosition
    {
        public ButtonPosition(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
