namespace DuskProject.Source.Creatures
{
    using Newtonsoft.Json;

    public class CreatureBase
    {
        [JsonProperty]
        public int X { get; set; } = 1;

        [JsonProperty]
        public int Y { get; set; } = 1;

        [JsonProperty]
        public int HP { get; protected set; } = 25;

        [JsonProperty]
        public int MaxHP { get; set; } = 25;

        [JsonProperty]
        public int MP { get; protected set; } = 4;

        [JsonProperty]
        public int MaxMP { get; set; } = 4;

        public virtual void AddHP(int amount)
        {
            HP += amount;

            if (HP < 0)
            {
                HP = 0;
            }

            if (HP > MaxHP)
            {
                HP = MaxHP;
            }
        }

        public virtual void AddMP(int amount)
        {
            MP += amount;

            if (MP < 0)
            {
                MP = 0;
            }

            if (MP > MaxMP)
            {
                MP = MaxMP;
            }
        }
    }
}
