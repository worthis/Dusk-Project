namespace DuskProject.Source
{
    using System;
    using DuskProject.Source.Dialog;
    using DuskProject.Source.Enums;
    using DuskProject.Source.Maze;

    public class Avatar
    {
        private static Avatar instance;
        private static object instanceLock = new object();

        private WindowManager _windowManager;
        private SoundManager _soundManager;
        private MazeWorldManager _mazeWorldManager;

        private AvatarFacing _facing;
        private bool _moved;
        private int _hp;
        private int _maxHp;
        private int _mp;
        private int _maxMp;
        private int _attack;
        private int _defence;
        private List<Item> _spellBook = new List<Item>();

        private Avatar()
        {
            Reset();
        }

        public int PosX { get; set; } = 1;

        public int PosY { get; set; } = 1;

        public bool Moved { get => _moved; }

        public AvatarFacing Facing { get => _facing; }

        public string MazeWorld { get; set; } = "0-serf-quarters";

        public string SleepMazeWorld { get; private set; } = "0-serf-quarters";

        public int SleepPosX { get; private set; } = 1;

        public int SleepPosY { get; private set; } = 1;

        public int Gold { get; private set; } = 0;

        public Item Weapon { get; private set; }

        public Item Armor { get; private set; }

        public int SpellBookLevel { get; set; } = 0;

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
            _facing = AvatarFacing.South;
            _moved = false;
            _hp = 25;
            _maxHp = 25;
            _mp = 4;
            _maxMp = 4;
            _attack = 0;
            _defence = 0;
            Gold = 0;
            SpellBookLevel = 0;
            _spellBook.Clear();

            SleepMazeWorld = "0-serf-quarters";
            SleepPosX = 1;
            SleepPosY = 1;
        }

        public void Sleep()
        {
            _hp = _maxHp;
            _mp = _maxMp;

            SleepMazeWorld = MazeWorld;
            SleepPosX = PosX;
            SleepPosY = PosY;
        }

        public void Respawn()
        {
            Gold = 0;

            _hp = _maxHp;
            _mp = _maxMp;

            PosX = SleepPosX;
            PosY = SleepPosY;

            _mazeWorldManager.LoadMazeWorld(SleepMazeWorld);
        }

        public bool IsBadlyHurt()
        {
            return _hp <= (int)(_maxHp / 3);
        }

        public void LearnSpell(Item spell)
        {
            if (spell is null)
            {
                return;
            }

            _spellBook.Add(spell);

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
                return _spellBook.Contains(item);
            }

            return false;
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
            return _hp.Equals(_maxHp) && _mp.Equals(_maxMp);
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
                _moved = true;

                return;
            }

            _soundManager.PlaySound(SFX.Blocked);
        }

        public void StepForward()
        {
            switch (_facing)
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
            switch (_facing)
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
            switch (_facing)
            {
                case AvatarFacing.North:
                    _facing = AvatarFacing.West;
                    break;

                case AvatarFacing.South:
                    _facing = AvatarFacing.East;
                    break;

                case AvatarFacing.East:
                    _facing = AvatarFacing.North;
                    break;

                case AvatarFacing.West:
                    _facing = AvatarFacing.South;
                    break;
            }
        }

        public void TurnRight()
        {
            switch (_facing)
            {
                case AvatarFacing.North:
                    _facing = AvatarFacing.East;
                    break;

                case AvatarFacing.South:
                    _facing = AvatarFacing.West;
                    break;

                case AvatarFacing.East:
                    _facing = AvatarFacing.South;
                    break;

                case AvatarFacing.West:
                    _facing = AvatarFacing.North;
                    break;
            }
        }

        public void Update()
        {
            _moved = false;

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
    }
}
