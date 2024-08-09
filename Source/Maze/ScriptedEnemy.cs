namespace DuskProject.Source.Maze
{
    public record ScriptedEnemy : MazePointBase
    {
        public string UniqueId { get; set; } = string.Empty;

        public string EnemyId { get; set; } = string.Empty;
    }
}
