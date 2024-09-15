namespace DuskProject.Source.GameStates.Combat.Phases;

using DuskProject.Source.Enums;

public class CombatDefeatPhase : CombatPhaseBase
{
    public CombatDefeatPhase(CombatState state)
        : base(state)
    {
    }

    public override void Render()
    {
        Combat.Enemy.Render();
        Combat.RenderOffenceLog();
        Combat.RenderDefenceLog();
        Combat.RenderHeroStats(true);
        Combat.TextManager.Render("You are defeated!", 316, 200, TextJustify.JUSTIFY_RIGHT);
    }

    public override void Update()
    {
        if (Combat.WindowManager.KeyPressed(InputKey.KEY_A) ||
                Combat.WindowManager.KeyPressed(InputKey.KEY_B))
        {
            Combat.Avatar.Respawn();
            _ = Combat.GameStateManager.Save();
            Combat.WorldManager.LoadWorld(Combat.Avatar.World);
            Combat.WorldManager.InitScriptedEvents(Combat.Avatar.HasCampaignFlag);
            Combat.GameStateManager.StartExplore();
        }
    }
}
