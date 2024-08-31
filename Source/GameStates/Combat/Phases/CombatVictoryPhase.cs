namespace DuskProject.Source.GameStates.Combat.Phases;

using DuskProject.Source.Enums;

public class CombatVictoryPhase : CombatPhaseBase
{
    public CombatVictoryPhase(CombatState state)
        : base(state)
    {
    }

    public override void Render()
    {
        Combat.RenderOffenceLog();
        Combat.RenderHeroStats(true);
        Combat.TextManager.Render("Victory!", 160, 120, TextJustify.JUSTIFY_CENTER);
        Combat.TextManager.Render(Combat.Reward.Result, 160, 140, TextJustify.JUSTIFY_CENTER);

        // todo: render gold coins
    }

    public override void Update()
    {
        if (Combat.WindowManager.KeyPressed(InputKey.KEY_A) ||
                Combat.WindowManager.KeyPressed(InputKey.KEY_B))
        {
            Combat.GameStateManager.StartExplore();
        }
    }
}
