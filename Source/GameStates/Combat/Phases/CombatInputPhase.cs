namespace DuskProject.Source.GameStates.Combat.Phases;

using DuskProject.Source.Creatures;
using DuskProject.Source.Enums;

public class CombatInputPhase : CombatPhaseBase
{
    private readonly Random _randGen;

    public CombatInputPhase(CombatState state)
        : base(state)
    {
        // Miyoo Mini Plus does not have RTC time
        _randGen = new Random(Environment.TickCount);
    }

    public override void Render()
    {
        Combat.Enemy.Render();
        Combat.RenderHeroStats();
        Combat.RenderOffenceLog();
        Combat.RenderDefenceLog();
        Combat.RenderActionButtons();
    }

    public override void Update()
    {
        bool usedAction = false;

        Combat.EnemyHurt = false;
        Combat.HeroHurt = false;
        Combat.RunSuccess = false;

        // Action selection
        if (Combat.WindowManager.KeyPressed(InputKey.KEY_DOWN) ||
            Combat.WindowManager.KeyPressed(InputKey.KEY_RIGHT))
        {
            var buttons = Combat.ActionButtons
                .Where(x => x.Enabled)
                .ToList();

            var button = buttons.Where(x => x.Selected).First();
            button.Selected = false;

            var index = buttons.IndexOf(button);
            if (index == buttons.Count - 1)
            {
                buttons.First().Selected = true;
            }
            else
            {
                buttons[index + 1].Selected = true;
            }
        }

        if (Combat.WindowManager.KeyPressed(InputKey.KEY_UP) ||
            Combat.WindowManager.KeyPressed(InputKey.KEY_LEFT))
        {
            var buttons = Combat.ActionButtons
                .Where(x => x.Enabled)
                .ToList();

            var button = buttons.Where(x => x.Selected).First();
            button.Selected = false;

            var index = buttons.IndexOf(button);
            if (index == 0)
            {
                buttons.Last().Selected = true;
            }
            else
            {
                buttons[index - 1].Selected = true;
            }
        }

        // Attack
        if (Combat.WindowManager.KeyPressed(InputKey.KEY_A))
        {
            usedAction = true;
            var buttonSelected = Combat.ActionButtons.Where(x => x.Selected).First();

            switch (buttonSelected.Action)
            {
                case ActionType.Attack:
                    DoAttackAction();
                    break;

                case ActionType.Run:
                    DoRunAction();
                    break;

                case ActionType.Heal:
                    DoHealAction();
                    break;

                case ActionType.Burn:
                    DoBurnAction();
                    break;

                case ActionType.Unlock:
                    DoUnlockAction();
                    break;

                default:
                    usedAction = false;
                    break;
            }
        }

        // Run
        if (Combat.WindowManager.KeyPressed(InputKey.KEY_B))
        {
            usedAction = true;
            DoRunAction();
        }

        if (usedAction)
        {
            Combat.Timer = 30;
            Combat.Phase = new CombatOffencePhase(Combat);
        }
    }

    private void DoAttackAction()
    {
        // Boss bone shield
        if (Combat.Enemy is EnemyBoss &&
            (Combat.Enemy as EnemyBoss).BoneShieldActive)
        {
            Combat.Offence = ("Attack!", "Absorbed!");
            Combat.SoundManager.PlaySound(SoundFX.Blocked);
            return;
        }

        // Check Miss
        if (_randGen.Next(100) < 20)
        {
            Combat.Offence = ("Attack!", "Miss!");
            Combat.SoundManager.PlaySound(SoundFX.Miss);
            return;
        }

        // Hit
        var attackDamage = _randGen.Next(Combat.Avatar.Weapon.AttackDispersion() + 1) + Combat.Avatar.Weapon.AttackMin + Combat.Avatar.Attack;

        // Critical Hit
        if (_randGen.Next(100) < 10)
        {
            attackDamage += Combat.Avatar.Weapon.AttackMax;
            Combat.Offence = ("Critical!", string.Format("{0} damage", attackDamage));
            Combat.SoundManager.PlaySound(SoundFX.Critical);
        }
        else
        {
            Combat.Offence = ("Attack!", string.Format("{0} damage", attackDamage));
            Combat.SoundManager.PlaySound(SoundFX.Attack);
        }

        Combat.EnemyHurt = true;
        Combat.Enemy.AddHP(-attackDamage);
    }

    private void DoRunAction()
    {
        if (_randGen.Next(100) < 66)
        {
            Combat.RunSuccess = true;
            Combat.Offence = ("Run!", string.Empty);
            Combat.SoundManager.PlaySound(SoundFX.Run);
            return;
        }

        Combat.Offence = ("Run!", "Blocked!");
        Combat.SoundManager.PlaySound(SoundFX.Blocked);
    }

    private void DoHealAction()
    {
        if (Combat.Avatar.MP <= 0 ||
            Combat.Avatar.HP >= Combat.Avatar.MaxHP)
        {
            return;
        }

        int healAmount = _randGen.Next((int)(Combat.Avatar.MaxHP * 0.5)) + (int)(Combat.Avatar.MaxHP * 0.5);
        Combat.Avatar.AddHP(healAmount);
        Combat.Avatar.AddMP(-1);

        Combat.Offence = ("Heal!", string.Format("+{0} HP", healAmount));
        Combat.SoundManager.PlaySound(SoundFX.Heal);
    }

    private void DoBurnAction()
    {
        if (Combat.Avatar.MP <= 0 ||
            !Combat.Avatar.KnowsSpell("Burn"))
        {
            return;
        }

        var attackDamage = _randGen.Next(Combat.Avatar.Weapon.AttackDispersion() + 1) + Combat.Avatar.Weapon.AttackMin + Combat.Avatar.Attack;

        // Against Undead burn does 2x crit
        if (Combat.Enemy.Category.Equals(EnemyCategory.Undead))
        {
            attackDamage += Combat.Avatar.Weapon.AttackMax;
            attackDamage += Combat.Avatar.Weapon.AttackMax;
        }

        // Against most creatures burn does 1x crit
        // Against demons burn does regular weapon damage
        if (!Combat.Enemy.Category.Equals(EnemyCategory.Demon))
        {
            attackDamage += Combat.Avatar.Weapon.AttackMax;
        }

        // Burn boneshield of the boss
        if (Combat.Enemy is EnemyBoss)
        {
            (Combat.Enemy as EnemyBoss).BurnBoneShield();
        }

        Combat.Avatar.AddMP(-1);

        Combat.EnemyHurt = true;
        Combat.Enemy.AddHP(-attackDamage);
        Combat.Offence = ("Burn!", string.Format("{0} damage", attackDamage));
        Combat.SoundManager.PlaySound(SoundFX.Fire);
    }

    private void DoUnlockAction()
    {
        if (Combat.Avatar.MP <= 0 ||
        !Combat.Avatar.KnowsSpell("Unlock") ||
            !Combat.Enemy.Category.Equals(EnemyCategory.Automaton))
        {
            return;
        }

        var spell = Combat.Avatar.GetSpell("Unlock");
        var attackDamage = _randGen.Next(Combat.Avatar.Weapon.AttackDispersion() + 1) + Combat.Avatar.Weapon.AttackMin + Combat.Avatar.Attack;

        // Unlock can only be cast against Automatons
        // so apply the full damage
        attackDamage += Combat.Avatar.Weapon.AttackMax;
        attackDamage += Combat.Avatar.Weapon.AttackMax;

        Combat.Avatar.AddMP(-1);

        Combat.EnemyHurt = true;
        Combat.Enemy.AddHP(-attackDamage);
        Combat.Offence = ("Unlock!", string.Format("{0} damage", attackDamage));
        Combat.SoundManager.PlaySound(SoundFX.Unlock);
    }
}
