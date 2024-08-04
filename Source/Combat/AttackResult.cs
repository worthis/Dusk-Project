namespace DuskProject.Source.Combat
{
    using DuskProject.Source.Enums;

    public record AttackResult
    {
        public string Action { get; set; } = string.Empty;

        public string Result { get; set; } = string.Empty;

        public SoundFX Sound { get; set; }

        public bool IsHeroDamaged { get; set; } = false;

        public int DamageToHeroHP { get; set; } = 0;

        public int DamageToHeroMP { get; set; } = 0;

        public int DamageToEnemyHP { get; set; } = 0;
    }
}
