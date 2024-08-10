namespace DuskProject.Source.Combat
{
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;

    public class Enemy : EnemyBase
    {
        private Random _randGen;

        public Enemy()
        {
            // Miyoo Mini Plus does not have RTC time
            _randGen = new Random(Environment.TickCount);
        }

        public int RenderOffsetX { get; set; } = 0;

        public int RenderOffsetY { get; set; } = 0;

        public ImageResource Image { get; set; }

        protected Random RandGen { get => _randGen; }

        public int AttackDispersion()
        {
            return AttackMax - AttackMin;
        }

        public int GoldDispersion()
        {
            return GoldMax - GoldMin;
        }

        public void Hit(int attackPoints)
        {
            HP -= attackPoints;

            if (HP <= 0)
            {
                HP = 0;
            }
        }

        public virtual AttackResult Attack(int heroDefence, CombatPowers combatPower)
        {
            switch (combatPower)
            {
                default:
                case CombatPowers.Attack:
                    {
                        // Miss
                        if (_randGen.Next(100) < 30)
                        {
                            return new AttackResult { Action = "Attack!", Result = "Miss!", Sound = SoundFX.Miss };
                        }

                        var attackDamage = _randGen.Next(AttackDispersion() + 1) + AttackMin;

                        // Critical Hit
                        if (_randGen.Next(100) < 5)
                        {
                            attackDamage += AttackMin;
                            attackDamage -= heroDefence;
                            if (attackDamage <= 0)
                            {
                                attackDamage = 1;
                            }

                            return new AttackResult
                            {
                                Action = "Critical!",
                                Result = string.Format("{0} damage", attackDamage),
                                IsHeroDamaged = true,
                                DamageToHeroHP = attackDamage,
                                Sound = SoundFX.Critical,
                            };
                        }

                        // Hit
                        attackDamage -= heroDefence;
                        if (attackDamage <= 0)
                        {
                            attackDamage = 1;
                        }

                        return new AttackResult
                        {
                            Action = "Attack!",
                            Result = string.Format("{0} damage", attackDamage),
                            IsHeroDamaged = true,
                            DamageToHeroHP = attackDamage,
                            Sound = SoundFX.Attack,
                        };
                    }

                case CombatPowers.Scorch:
                    {
                        // Miss
                        if (_randGen.Next(100) < 30)
                        {
                            return new AttackResult { Action = "Scorch!", Result = "Miss!", Sound = SoundFX.Miss };
                        }

                        var attackDamage = _randGen.Next(AttackDispersion() + 1) + AttackMin;
                        attackDamage += AttackMin; // Scorch works like critical hit
                        attackDamage -= heroDefence;
                        if (attackDamage <= 0)
                        {
                            attackDamage = 1;
                        }

                        // Hit
                        return new AttackResult
                        {
                            Action = "Scorch!",
                            Result = string.Format("{0} damage", attackDamage),
                            IsHeroDamaged = true,
                            DamageToHeroHP = attackDamage,
                            Sound = SoundFX.Fire,
                        };
                    }

                case CombatPowers.HPDrain:
                    {
                        // Miss
                        if (_randGen.Next(100) < 30)
                        {
                            return new AttackResult { Action = "HP Drain!", Result = "Miss!", Sound = SoundFX.Miss };
                        }

                        var attackDamage = _randGen.Next(AttackDispersion() + 1) + AttackMin;
                        attackDamage -= heroDefence;
                        if (attackDamage <= 0)
                        {
                            attackDamage = 1;
                        }

                        // Hit
                        return new AttackResult
                        {
                            Action = "HP Drain!",
                            Result = string.Format("{0} damage", attackDamage),
                            IsHeroDamaged = true,
                            DamageToHeroHP = attackDamage,
                            DamageToEnemyHP = -attackDamage,
                            Sound = SoundFX.HPDrain,
                        };
                    }

                case CombatPowers.MPDrain:
                    {
                        // Miss
                        if (_randGen.Next(100) < 30)
                        {
                            return new AttackResult { Action = "MP Drain!", Result = "Miss!", Sound = SoundFX.Miss };
                        }

                        // Hit
                        return new AttackResult
                        {
                            Action = "MP Drain!",
                            Result = "-1 MP",
                            IsHeroDamaged = true,
                            DamageToHeroMP = 1,
                            Sound = SoundFX.MPDrain,
                        };
                    }
            }
        }

        public virtual AttackResult Attack(int heroDefence)
        {
            var powerId = _randGen.Next(Powers.Count);
            return Attack(heroDefence, Powers[powerId]);
        }

        public int GoldReward()
        {
            return _randGen.Next(GoldDispersion()) + GoldMin;
        }

        public virtual void Render()
        {
            Image.Render(
                0,
                0,
                Image.Width,
                Image.Height,
                RenderOffsetX,
                RenderOffsetY);
        }
    }
}
