namespace DuskProject.Source.Dialog
{
    using DuskProject.Source.Enums;

    public class Item
    {
        public string Name { get; set; } = string.Empty;

        public ItemType Type { get; set; }

        public int Gold { get; set; } = 0;

        public int AttackMin { get; set; } = 0;

        public int AttackMax { get; set; } = 0;

        public int Defence { get; set; } = 0;

        public int Level { get; set; } = 0;

        public int AttackAvg()
        {
            return (int)((AttackMin + AttackMax) * 0.5);
        }
    }
}
