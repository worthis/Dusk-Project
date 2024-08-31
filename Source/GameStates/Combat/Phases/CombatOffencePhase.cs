namespace DuskProject.Source.GameStates.Combat.Phases;

using DuskProject.Source.Combat;
using DuskProject.Source.Enums;

public class CombatOffencePhase : CombatPhaseBase
{
    private readonly Random _randGen;

    public CombatOffencePhase(CombatState state)
        : base(state)
    {
        // Miyoo Mini Plus does not have RTC time
        _randGen = new Random(Environment.TickCount);
    }

    public override void Render()
    {
        Combat.Enemy.Render();

        if (Combat.Timer <= 25)
        {
            Combat.RenderOffenceLog();
        }
    }

    public override void Update()
    {
        Combat.Timer--;

        if (Combat.Timer > 15 &&
            Combat.EnemyHurt)
        {
            Combat.Enemy.RenderOffsetX = _randGen.Next(8) - 4;
            Combat.Enemy.RenderOffsetY = _randGen.Next(8) - 4;
        }

        if (Combat.Timer == 15)
        {
            Combat.Enemy.RenderOffsetX = 0;
            Combat.Enemy.RenderOffsetY = 0;
        }

        if (Combat.Timer <= 0)
        {
            if (Combat.Enemy.HP <= 0)
            {
                Combat.Phase = new CombatVictoryPhase(Combat);
                Combat.SoundManager.PlaySound(SoundFX.Coin);
                GiveReward();
                return;
            }

            if (Combat.RunSuccess)
            {
                Combat.GameStateManager.StartExplore();
                return;
            }

            // Enemy attack
            AttackResult attackResult = Combat.Enemy.Attack(Combat.Avatar.Armor.Defence + Combat.Avatar.Defence);

            Combat.Defence = (attackResult.Action, attackResult.Result);
            Combat.SoundManager.PlaySound(attackResult.Sound);
            Combat.HeroHurt = attackResult.IsHeroDamaged;

            Combat.Avatar.AddHP(-attackResult.DamageToHeroHP);
            Combat.Avatar.AddMP(-attackResult.DamageToHeroMP);
            Combat.Enemy.AddHP(-attackResult.DamageToEnemyHP);

            Combat.Timer = 30;
            Combat.Phase = new CombatDefencePhase(Combat);
        }
    }

    private void GiveReward()
    {
        var goldReward = Combat.Enemy.GoldReward();
        Combat.Avatar.AddGold(goldReward);
        Combat.Avatar.PushCampaignFlag(Combat.GameStateManager.Enemy.UniqueFlag);
        Combat.Reward = (goldReward, string.Format("{0} Gold!", goldReward));
    }
}
