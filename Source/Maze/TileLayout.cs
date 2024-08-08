﻿namespace DuskProject.Source.Maze
{
    public record TileLayout
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int SrcX { get; set; }

        public int SrcY { get; set; }

        public int DstX { get; set; }

        public int DstY { get; set; }
    }
}
