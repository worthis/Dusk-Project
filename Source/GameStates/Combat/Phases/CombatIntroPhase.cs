namespace DuskProject.Source.GameStates.Combat.Phases;

public class CombatIntroPhase : CombatPhaseBase
{
    public CombatIntroPhase(CombatState combatState)
        : base(combatState)
    {
    }

    public override void Render()
    {
        if (Combat.Timer < 15)
        {
            Combat.Enemy.Render();
        }
    }

    public override void Update()
    {
        Combat.Timer--;
        Combat.Enemy.RenderOffsetX = 0 - (Combat.Timer * 20);

        if (Combat.Timer <= 0)
        {
            Combat.Phase = new CombatInputPhase(Combat);
        }
    }
}
