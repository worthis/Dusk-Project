namespace DuskProject.Source.World
{
    public record ScriptedEnemy : WorldPoint
    {
        public string UniqueId { get; set; } = string.Empty;

        public string EnemyId { get; set; } = string.Empty;
    }
}
