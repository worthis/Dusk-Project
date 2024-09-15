namespace DuskProject.Source.GameStates.Combat.Phases;

using DuskProject.Source.Enums;

public class CombatDefencePhase : CombatPhaseBase
{
    private readonly Random _randGen;

    public CombatDefencePhase(CombatState state)
        : base(state)
    {
        // Miyoo Mini Plus does not have RTC time
        _randGen = new Random(Environment.TickCount);
    }

    public override void Render()
    {
        Combat.Enemy.Render();
        Combat.RenderOffenceLog();
        Combat.RenderDefenceLog();
    }

    public override void Update()
    {
        Combat.Timer--;

        if (Combat.Timer > 15 &&
            Combat.HeroHurt)
        {
            Combat.WorldManager.TileSetRenderOffsetX = _randGen.Next(8) - 4;
            Combat.WorldManager.TileSetRenderOffsetY = _randGen.Next(8) - 4;
        }

        if (Combat.Timer == 15)
        {
            Combat.WorldManager.TileSetRenderOffsetX = 0;
            Combat.WorldManager.TileSetRenderOffsetY = 0;
        }

        if (Combat.Timer <= 0)
        {
            // Check Defeat
            if (Combat.Avatar.HP <= 0)
            {
                Combat.SoundManager.PlaySound(SoundFX.Defeat);
                Combat.Phase = new CombatDefeatPhase(Combat);
                return;
            }

            Combat.Phase = new CombatInputPhase(Combat);
        }
    }
}
