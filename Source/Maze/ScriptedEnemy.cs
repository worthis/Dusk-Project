namespace DuskProject.Source.Maze
{
    public record ScriptedEnemy
    {
        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public string UniqueId { get; set; } = string.Empty;

        public string EnemyId { get; set; } = string.Empty;

        public bool CheckEnter(int posX, int posY)
        {
            return posX == X && posY == Y;
        }
    }
}
