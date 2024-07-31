namespace DuskProject.Source
{
    using System;
    using DuskProject.Source.MazeObjects;

    public enum AvatarFacing : byte
    {
        North = 0,
        South = 1,
        West = 2,
        East = 3,
    }

    public class Avatar
    {
        private static Avatar instance;
        private static object instanceLock = new object();

        private WindowManager _windowManager;
        private MazeWorldManager _mazeWorldManager;

        private int _posX;
        private int _posY;
        private AvatarFacing _facing;
        private bool _moved;
        private int _weapon;
        private int _armour;
        private int _hp;
        private int _maxHp;
        private int _mp;
        private int _maxMp;
        private int _attack;
        private int _defence;
        private int _spellBook;
        private int _gold;

        private string _sleepMazeWorld;
        private int _sleepPosX;
        private int _sleepPosY;

        private Avatar()
        {
            Init();
            Reset();
        }

        public int PosX { get => _posX; set => _posX = value; }

        public int PosY { get => _posY; set => _posY = value; }

        public bool Moved { get => _moved; }

        public AvatarFacing Facing { get => _facing; }

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
            _mazeWorldManager = MazeWorldManager.GetInstance();
        }

        public void Reset()
        {
            _posX = 1;
            _posY = 1;
            _facing = AvatarFacing.South;
            _moved = false;
            _weapon = 0;
            _armour = 1;
            _hp = 25;
            _maxHp = 25;
            _mp = 4;
            _maxMp = 4;
            _attack = 0;
            _defence = 0;
            _spellBook = 0;
            _gold = 0;

            _sleepMazeWorld = "0-serf-quarters";
            _sleepPosX = 1;
            _sleepPosY = 1;
        }

        public void Sleep(string mazeWorldName, int posX, int posY)
        {
            _hp = _maxHp;
            _mp = _maxMp;

            _sleepMazeWorld = mazeWorldName;
            _sleepPosX = posX;
            _sleepPosY = posY;
        }

        public void Respawn()
        {
            _hp = _maxHp;
            _mp = _maxMp;

            _posX = _sleepPosX;
            _posY = _sleepPosY;

            _gold = 0;

            // loadworld
        }

        public bool IsBadlyHurt()
        {
            return _hp <= (int)(_maxHp / 3);
        }

        public void Move(int dX, int dY)
        {
            Tile tile = _mazeWorldManager.GetTile(_posX + dX, _posY + dY);

            if (tile is not null &&
                tile.Walkable)
            {
                _posX += dX;
                _posY += dY;
                _moved = true;

                return;
            }

            // play sound - blocked
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

            if (_windowManager.KeyPressed(InputKeys.KEY_UP))
            {
                StepForward();
            }

            if (_windowManager.KeyPressed(InputKeys.KEY_DOWN))
            {
                StepBackward();
            }

            if (_windowManager.KeyPressed(InputKeys.KEY_LEFT))
            {
                TurnLeft();
            }

            if (_windowManager.KeyPressed(InputKeys.KEY_RIGHT))
            {
                TurnRight();
            }
        }
    }
}
