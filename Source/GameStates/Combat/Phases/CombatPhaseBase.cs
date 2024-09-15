namespace DuskProject.Source.GameStates.Combat.Phases;

public abstract class CombatPhaseBase
{
    private readonly CombatState _combat;

    public CombatPhaseBase(CombatState state)
    {
        _combat = state;
    }

    protected CombatState Combat { get => _combat; }

    public abstract void Render();

    public abstract void Update();
}
