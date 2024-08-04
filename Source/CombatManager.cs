namespace DuskProject.Source
{
    using System;
    using DuskProject.Source.Combat;
    using DuskProject.Source.Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class CombatManager
    {
        private static CombatManager instance;
        private static object instanceLock = new object();

        private WindowManager _windowManager;
        private GameStateManager _gameStateManager;
        private ResourceManager _resourceManager;
        private SoundManager _soundManager;
        private TextManager _textManager;
        private MazeWorldManager _mazeWorldManager;
        private Avatar _avatar;

        private Random _randGen;

        private Enemy _enemy;
        private string _uniqueFlag = string.Empty;
        private CombatPhase _phase = CombatPhase.Intro;
        private int _timer = 0;

        private string _offenceAction = string.Empty;
        private string _offenceResult = string.Empty;
        private string _defenceAction = string.Empty;
        private string _defenceResult = string.Empty;
        private string _rewardResult = string.Empty;
        private int _rewardGoldAmount = 0;
        private bool _enemyHurt = false;
        private bool _heroHurt = false;
        private bool _runSuccess = false;

        private CombatManager()
        {
        }

        public static CombatManager GetInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CombatManager();
                        Console.WriteLine("CombatManager created");
                    }
                }
            }

            return instance;
        }

        public void Init()
        {
            _windowManager = WindowManager.GetInstance();
            _gameStateManager = GameStateManager.GetInstance();
            _resourceManager = ResourceManager.GetInstance();
            _soundManager = SoundManager.GetInstance();
            _textManager = TextManager.GetInstance();
            _mazeWorldManager = MazeWorldManager.GetInstance();
            _avatar = Avatar.GetInstance();

            // Miyoo Mini Plus does not have RTC time
            _randGen = new Random(Environment.TickCount);

            Console.WriteLine("CombatManager initialized");
        }

        public void StartCombat(string enemyId, string uniqueFlag = "")
        {
            Reset();
            LoadEnemy(enemyId);

            _uniqueFlag = uniqueFlag;
            _phase = CombatPhase.Intro;
            _timer = 15;
        }

        public void Update()
        {
            _textManager.Color = _avatar.IsBadlyHurt() ? TextColor.Red : TextColor.Default;

            switch (_phase)
            {
                case CombatPhase.Intro:
                    UpdateIntro();
                    break;

                case CombatPhase.Input:
                    UpdateInput();
                    break;

                case CombatPhase.Offence:
                    UpdateOffence();
                    break;

                case CombatPhase.Defence:
                    UpdateDefence();
                    break;

                case CombatPhase.Victory:
                    UpdateVictory();
                    break;

                case CombatPhase.Defeat:
                    UpdateDefeat();
                    break;
            }
        }

        public void Render()
        {
            // Maze Cell
            _mazeWorldManager.RenderBackground(_avatar.Facing);
            _mazeWorldManager.Render(_avatar.PosX, _avatar.PosY, _avatar.Facing);

            switch (_phase)
            {
                case CombatPhase.Intro:
                    if (_timer < 15)
                    {
                        _enemy.Render();
                    }

                    break;

                case CombatPhase.Input:
                    _enemy.Render();
                    RenderHeroStats();
                    RenderOffenceLog();
                    RenderDefenceLog();
                    break;

                case CombatPhase.Offence:
                    _enemy.Render();

                    if (_timer <= 25)
                    {
                        RenderOffenceLog();
                    }

                    break;

                case CombatPhase.Defence:
                    _enemy.Render();
                    RenderOffenceLog();
                    RenderDefenceLog();
                    break;

                case CombatPhase.Victory:
                    RenderOffenceLog();
                    RenderHeroStats(true);
                    _textManager.Render("Victory!", 160, 120, TextJustify.JUSTIFY_CENTER);
                    _textManager.Render(_rewardResult, 160, 140, TextJustify.JUSTIFY_CENTER);

                    // todo: render gold coins
                    break;

                case CombatPhase.Defeat:
                    _enemy.Render();
                    RenderOffenceLog();
                    RenderDefenceLog();
                    RenderHeroStats(true);
                    _textManager.Render("You are defeated!", 316, 200, TextJustify.JUSTIFY_RIGHT);
                    break;
            }

            _textManager.Render(_enemy.Name, 160, 4, TextJustify.JUSTIFY_CENTER);
        }

        private void UpdateIntro()
        {
            _timer--;
            _enemy.RenderOffsetX = 0 - (_timer * 20);

            if (_timer <= 0)
            {
                _phase = CombatPhase.Input;
            }
        }

        private void UpdateInput()
        {
            bool usedAction = false;

            _enemyHurt = false;
            _heroHurt = false;
            _runSuccess = false;

            // Attack
            if (_windowManager.KeyPressed(InputKey.KEY_A))
            {
                usedAction = true;
                _offenceAction = "Attack!";

                /* todo: boss special */

                // Check Miss
                if (_randGen.Next(100) < 20)
                {
                    _offenceResult = "Miss!";
                    _soundManager.PlaySound(SFX.Miss);
                }
                else
                {
                    // Hit
                    var attackDamage = _randGen.Next(_avatar.Weapon.AttackDispersion() + 1) + _avatar.Weapon.AttackMin + _avatar.Attack;

                    // Critical Hit
                    if (_randGen.Next(100) < 10)
                    {
                        attackDamage += _avatar.Weapon.AttackMax;
                        _offenceAction = "Critical!";
                        _soundManager.PlaySound(SFX.Critical);
                    }
                    else
                    {
                        _soundManager.PlaySound(SFX.Attack);
                    }

                    _enemyHurt = true;
                    _enemy.HP -= attackDamage;
                    _offenceResult = string.Format("{0} damage", attackDamage);
                }
            }

            // Run
            if (_windowManager.KeyPressed(InputKey.KEY_B))
            {
                usedAction = true;

                _offenceAction = "Run!";
                _soundManager.PlaySound(SFX.Run);

                if (_randGen.Next(100) < 66)
                {
                    _runSuccess = true;
                    _offenceResult = string.Empty;
                }
                else
                {
                    _offenceResult = "Blocked!";
                }
            }

            // todo: Spells
            {
            }

            if (usedAction)
            {
                _timer = 30;
                _phase = CombatPhase.Offence;
            }
        }

        private void UpdateOffence()
        {
            _timer--;

            if (_timer > 15 &&
                _enemyHurt)
            {
                _enemy.RenderOffsetX = _randGen.Next(8) - 4;
                _enemy.RenderOffsetY = _randGen.Next(8) - 4;
            }

            if (_timer == 15)
            {
                _enemy.RenderOffsetX = 0;
                _enemy.RenderOffsetY = 0;
            }

            if (_timer <= 0)
            {
                if (_enemy.HP <= 0)
                {
                    _phase = CombatPhase.Victory;
                    _soundManager.PlaySound(SFX.Coin);
                    GiveReward();
                    return;
                }

                if (_runSuccess)
                {
                    Reset();
                    _gameStateManager.ChangeState(GameState.Explore);

                    return;
                }

                // Enemy attack
                AttackResult attackResult = _enemy.Attack(_avatar.Armor.Defence + _avatar.Defence);

                _defenceAction = attackResult.Action;
                _defenceResult = attackResult.Result;
                _soundManager.PlaySound(attackResult.Sound);
                _heroHurt = attackResult.IsHeroDamaged;

                _avatar.Hit(attackResult.DamageToEnemyHP);
                _avatar.DrainMP(attackResult.DamageToHeroMP);
                _enemy.Hit(attackResult.DamageToEnemyHP);

                _timer = 30;
                _phase = CombatPhase.Defence;
            }
        }

        private void UpdateDefence()
        {
            _timer--;

            if (_timer > 15 &&
                _heroHurt)
            {
                _mazeWorldManager.TileSetRenderOffsetX = _randGen.Next(8) - 4;
                _mazeWorldManager.TileSetRenderOffsetY = _randGen.Next(8) - 4;
            }

            if (_timer == 15)
            {
                _mazeWorldManager.TileSetRenderOffsetX = 0;
                _mazeWorldManager.TileSetRenderOffsetY = 0;
            }

            if (_timer <= 0)
            {
                // Check Defeat
                if (_avatar.HP <= 0)
                {
                    _phase = CombatPhase.Defeat;
                    _soundManager.PlaySound(SFX.Defeat);
                    return;
                }

                _phase = CombatPhase.Input;
            }
        }

        private void UpdateVictory()
        {
            if (_windowManager.KeyPressed(InputKey.KEY_A) ||
                _windowManager.KeyPressed(InputKey.KEY_B))
            {
                Reset();
                _gameStateManager.ChangeState(GameState.Explore);
            }
        }

        private void UpdateDefeat()
        {
            if (_windowManager.KeyPressed(InputKey.KEY_A) ||
                _windowManager.KeyPressed(InputKey.KEY_B))
            {
                Reset();
                _avatar.Respawn();
                _gameStateManager.ChangeState(GameState.Explore);
            }
        }

        private void RenderHeroStats(bool showGold = false)
        {
            _textManager.Render(string.Format("HP {0}/{1}", _avatar.HP, _avatar.MaxHP), 4, 200);
            _textManager.Render(string.Format("MP {0}/{1}", _avatar.MP, _avatar.MaxMP), 4, 220);

            if (showGold)
            {
                _textManager.Render(string.Format("{0} Gold", _avatar.Gold), 316, 220, TextJustify.JUSTIFY_RIGHT);
            }
        }

        private void RenderOffenceLog()
        {
            if (string.IsNullOrEmpty(_offenceAction))
            {
                return;
            }

            _textManager.Render("You:", 4, 40);
            _textManager.Render(_offenceAction, 4, 60);
            _textManager.Render(_offenceResult, 4, 80);
        }

        private void RenderDefenceLog()
        {
            if (string.IsNullOrEmpty(_defenceAction))
            {
                return;
            }

            _textManager.Render("Enemy:", 4, 120);
            _textManager.Render(_defenceAction, 4, 140);
            _textManager.Render(_defenceResult, 4, 160);
        }

        private void GiveReward()
        {
            var goldReward = _enemy.GoldReward();
            _avatar.AddGold(goldReward);
            _avatar.PushCampaignFlag(_uniqueFlag);
            _rewardResult = string.Format("{0} Gold!", goldReward);
            _rewardGoldAmount = goldReward;
        }

        private void LoadEnemy(string enemyId)
        {
            string fileName = string.Format("Data/enemies/{0}.json", enemyId);

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Error: Unable to load enemy {0} file {1}", enemyId, fileName);
                return;
            }

            using (StreamReader streamReader = new(fileName))
            {
                string jsonData = streamReader.ReadToEnd();
                streamReader.Close();

                _enemy = JsonConvert.DeserializeObject<Enemy>(jsonData, new StringEnumConverter());
                _enemy.Id = enemyId;
                _enemy.Image = _resourceManager.LoadImage(string.Format("Data/images/enemies/{0}", _enemy.ImageFile));
            }

            Console.WriteLine("Enemy {0} loaded from {1}", enemyId, fileName);
        }

        private void Reset()
        {
            _offenceAction = string.Empty;
            _offenceResult = string.Empty;
            _defenceAction = string.Empty;
            _defenceResult = string.Empty;
            _rewardResult = string.Empty;
            _enemyHurt = false;
            _heroHurt = false;
            _runSuccess = false;
        }
    }
}
