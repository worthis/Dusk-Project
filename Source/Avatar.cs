namespace DuskProject.Source
{
    using System;
    using DuskProject.Source.Dialog;
    using DuskProject.Source.Enums;
    using DuskProject.Source.Maze;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class Avatar : AvatarBase
    {
        private static Avatar instance;
        private static object instanceLock = new object();

        private WindowManager _windowManager;
        private SoundManager _soundManager;
        private MazeWorldManager _mazeWorldManager;

        private Avatar()
        {
            Reset();
        }

        public static Avatar GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new Avatar();
                        Console.WriteLine("Avatar created");
                    }
                }
            }

            return instance;
        }

        public void Init()
        {
            _windowManager = WindowManager.GetInstance();
            _soundManager = SoundManager.GetInstance();
            _mazeWorldManager = MazeWorldManager.GetInstance();

            Console.WriteLine("Avatar initialized");
        }

        public void Reset()
        {
            PosX = 1;
            PosY = 1;
            Facing = AvatarFacing.South;
            Moved = false;
            HP = 25;
            MaxHP = 25;
            MP = 4;
            MaxMP = 4;
            Attack = 0;
            Defence = 0;
            Gold = 0;
            SpellBookLevel = 0;
            SpellBook.Clear();

            SleepMazeWorld = "0-serf-quarters";
            SleepPosX = 1;
            SleepPosY = 1;
        }

        public void Save()
        {
            Directory.CreateDirectory("Save");

            using (StreamWriter streamWriter = new("Save/avatar.json"))
            {
                AvatarBase avatarBase = this;
                string avatarData = JsonConvert.SerializeObject(avatarBase, new StringEnumConverter());
                streamWriter.Write(avatarData);
            }
        }

        public bool SaveExists()
        {
            return File.Exists("Save/avatar.json");
        }

        public void Load()
        {
            if (!SaveExists())
            {
                return;
            }

            using (StreamReader streamReader = new("Save/avatar.json"))
            {
                string jsonData = streamReader.ReadToEnd();
                streamReader.Close();

                var avatarBase = JsonConvert.DeserializeObject<AvatarBase>(jsonData, new StringEnumConverter());

                if (avatarBase is not null)
                {
                    HP = avatarBase.HP;
                    MaxHP = avatarBase.MaxHP;
                    MP = avatarBase.MP;
                    MaxMP = avatarBase.MaxMP;
                    Attack = avatarBase.Attack;
                    Defence = avatarBase.Defence;
                    Gold = avatarBase.Gold;
                    PosX = avatarBase.PosX;
                    PosY = avatarBase.PosY;
                    Facing = avatarBase.Facing;
                    MazeWorld = avatarBase.MazeWorld;
                    SleepPosX = avatarBase.SleepPosX;
                    SleepPosY = avatarBase.SleepPosY;
                    SleepMazeWorld = avatarBase.SleepMazeWorld;
                    Weapon = avatarBase.Weapon;
                    Armor = avatarBase.Armor;
                    SpellBookLevel = avatarBase.SpellBookLevel;
                    SpellBook = avatarBase.SpellBook;
                    Campaign = avatarBase.Campaign;
                }
            }
        }

        public void Sleep()
        {
            HP = MaxHP;
            MP = MaxMP;

            SleepMazeWorld = MazeWorld;
            SleepPosX = PosX;
            SleepPosY = PosY;

            Save();
        }

        public void Respawn()
        {
            Gold = 0;

            HP = MaxHP;
            MP = MaxMP;

            PosX = SleepPosX;
            PosY = SleepPosY;

            _mazeWorldManager.LoadMazeWorld(SleepMazeWorld);
        }

        public void Hit(int attackPoints)
        {
            HP -= attackPoints;

            if (HP < 0)
            {
                HP = 0;
            }

            if (HP > MaxHP)
            {
                HP = MaxHP;
            }
        }

        public void Heal(int healPoints)
        {
            Hit(-healPoints);
        }

        public void DrainMP(int manaPoints)
        {
            MP -= manaPoints;

            if (MP < 0)
            {
                MP = 0;
            }

            if (MP > MaxMP)
            {
                MP = MaxMP;
            }
        }

        public bool IsBadlyHurt()
        {
            return HP <= (int)(MaxHP / 3);
        }

        public void LearnSpell(Item spell)
        {
            if (spell is null)
            {
                return;
            }

            SpellBook.Add(spell);

            if (SpellBookLevel < spell.Level)
            {
                SpellBookLevel = spell.Level;
            }
        }

        public void EquipItem(Item item)
        {
            if (item is null)
            {
                return;
            }

            switch (item.Type)
            {
                case ItemType.Weapon:
                    Weapon = item;
                    break;

                case ItemType.Armor:
                    Armor = item;
                    break;
            }
        }

        public bool HasItem(Item item)
        {
            if (item is null)
            {
                return false;
            }

            switch (item.Type)
            {
                case ItemType.Weapon:
                    return Weapon is not null &&
                        Weapon.Name.Equals(item.Name);

                case ItemType.Armor:
                    return Armor is not null &&
                        Armor.Name.Equals(item.Name);
            }

            return false;
        }

        public bool KnowsSpell(Item item)
        {
            if (item is null)
            {
                return false;
            }

            if (item.Type.Equals(ItemType.Spell))
            {
                return SpellBook.Contains(item);
            }

            return false;
        }

        public bool KnowsSpell(string spellName)
        {
            if (spellName is null)
            {
                return false;
            }

            foreach (var spell in SpellBook)
            {
                if (spell.Name.Equals(spellName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public Item GetSpell(string spellName)
        {
            if (spellName is null)
            {
                return null;
            }

            foreach (var spell in SpellBook)
            {
                if (spell.Name.Equals(spellName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return spell;
                }
            }

            return null;
        }

        public bool IsBetterItem(Item item)
        {
            if (item is null)
            {
                return false;
            }

            switch (item.Type)
            {
                case ItemType.Weapon:
                    return Weapon is null ||
                        (!Weapon.Name.Equals(item.Name) &&
                        Weapon.AttackAvg() <= item.AttackAvg());

                case ItemType.Armor:
                    return Armor is null ||
                        (!Armor.Name.Equals(item.Name) &&
                        Armor.Defence <= item.Defence);
            }

            return false;
        }

        public bool IsRested()
        {
            return HP.Equals(MaxHP) && MP.Equals(MaxMP);
        }

        public void AddGold(int gold)
        {
            Gold += gold;

            if (Gold < 0)
            {
                Gold = 0;
            }
        }

        public void Move(int dX, int dY)
        {
            Tile tile = _mazeWorldManager.GetTile(PosX + dX, PosY + dY);

            if (tile is not null &&
                tile.Walkable)
            {
                PosX += dX;
                PosY += dY;
                Moved = true;

                return;
            }

            _soundManager.PlaySound(SoundFX.Blocked);
        }

        public void StepForward()
        {
            switch (Facing)
            {
                case AvatarFacing.North:
                    Move(0, -1);
                    break;

                case AvatarFacing.South:
                    Move(0, 1);
                    break;

                case AvatarFacing.East:
                    Move(1, 0);
                    break;

                case AvatarFacing.West:
                    Move(-1, 0);
                    break;
            }
        }

        public void StepBackward()
        {
            switch (Facing)
            {
                case AvatarFacing.North:
                    Move(0, 1);
                    break;

                case AvatarFacing.South:
                    Move(0, -1);
                    break;

                case AvatarFacing.East:
                    Move(-1, 0);
                    break;

                case AvatarFacing.West:
                    Move(1, 0);
                    break;
            }
        }

        public void TurnLeft()
        {
            switch (Facing)
            {
                case AvatarFacing.North:
                    Facing = AvatarFacing.West;
                    break;

                case AvatarFacing.South:
                    Facing = AvatarFacing.East;
                    break;

                case AvatarFacing.East:
                    Facing = AvatarFacing.North;
                    break;

                case AvatarFacing.West:
                    Facing = AvatarFacing.South;
                    break;
            }
        }

        public void TurnRight()
        {
            switch (Facing)
            {
                case AvatarFacing.North:
                    Facing = AvatarFacing.East;
                    break;

                case AvatarFacing.South:
                    Facing = AvatarFacing.West;
                    break;

                case AvatarFacing.East:
                    Facing = AvatarFacing.South;
                    break;

                case AvatarFacing.West:
                    Facing = AvatarFacing.North;
                    break;
            }
        }

        public void Update()
        {
            Moved = false;

            if (_windowManager.KeyPressed(InputKey.KEY_UP))
            {
                StepForward();
            }

            if (_windowManager.KeyPressed(InputKey.KEY_DOWN))
            {
                StepBackward();
            }

            if (_windowManager.KeyPressed(InputKey.KEY_LEFT))
            {
                TurnLeft();
            }

            if (_windowManager.KeyPressed(InputKey.KEY_RIGHT))
            {
                TurnRight();
            }
        }

        public void PushCampaignFlag(string flag)
        {
            if (string.IsNullOrWhiteSpace(flag))
            {
                return;
            }

            if (HasCampaignFlag(flag))
            {
                Console.WriteLine("Campaign already has flag {0}", flag.ToUpper());
                return;
            }

            Campaign.Add(flag.ToUpper());
        }

        public void PopCampaignFlag(string flag)
        {
            if (string.IsNullOrWhiteSpace(flag))
            {
                return;
            }

            if (!HasCampaignFlag(flag))
            {
                Console.WriteLine("Campaign doesn't have flag {0}", flag.ToUpper());
                return;
            }

            Campaign.Remove(flag.ToUpper());
        }

        public bool HasCampaignFlag(string flag)
        {
            if (string.IsNullOrWhiteSpace(flag))
            {
                return false;
            }

            return Campaign.Contains(flag.ToUpper());
        }
    }
}
