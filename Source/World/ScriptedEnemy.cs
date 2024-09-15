namespace DuskProject.Source.World;

public record ScriptedEnemy : WorldPoint
{
    public string UniqueId { get; init; } = string.Empty;

    public string EnemyId { get; init; } = string.Empty;
}
