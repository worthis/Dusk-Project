namespace DuskProject.Source.Creatures
{
    using DuskProject.Source.Combat;
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;

    public class EnemyBoss : Enemy
    {
        private ImageResource _boneShield;
        private bool _boneShieldActive = false;
        private int _boneShieldCount = 0;

        public bool BoneShieldActive { get => _boneShieldActive; }

        public void InitBoneShield(ImageResource image)
        {
            _boneShield = image;
        }

        public override void Render()
        {
            if (_boneShieldActive)
            {
                _boneShield.Render();
            }

            base.Render();
        }

        public override AttackResult Attack(int heroDefence)
        {
            // Simple attack
            if (RandGen.Next(100) < 66)
            {
                return base.Attack(heroDefence);
            }

            // Scorch
            if (BoneShieldActive ||
                _boneShieldCount >= 3 ||
                RandGen.Next(100) < 33)
            {
                return Attack(heroDefence, CombatPowers.Scorch);
            }

            // BoneShield
            _boneShieldCount++;
            _boneShieldActive = true;
            return new AttackResult
            {
                Action = "Bone Shield!",
                Result = "+Def Up!",
                Sound = SoundFX.BoneShield,
            };
        }

        public void BurnBoneShield()
        {
            _boneShieldActive = false;
            _boneShieldCount = 0;
        }
    }
}
