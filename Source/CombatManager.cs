namespace DuskProject.Source
{
    using System;
    using DuskProject.Source.Combat;
    using DuskProject.Source.Creatures;
    using DuskProject.Source.Enums;
    using DuskProject.Source.Resources;
    using DuskProject.Source.UI;
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
        private ExploreManager _exploreManager;
        private Avatar _avatar;

        private Random _randGen;

        private Enemy _enemy;
        private string _uniqueFlag = string.Empty;
        private CombatPhase _phase = CombatPhase.Intro;
        private int _timer = 0;

        private List<InfoButton> _actionButtons = new List<InfoButton>();

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
            _exploreManager = ExploreManager.GetInstance();
            _avatar = Avatar.GetInstance();

            // Miyoo Mini Plus does not have RTC time
            _randGen = new Random(Environment.TickCount);

            ImageResource buttonsImage = _resourceManager.LoadImage("Data/images/interface/action_buttons.png");
            ImageResource buttonSelectedImage = _resourceManager.LoadImage("Data/images/interface/select.png");

            _actionButtons.Add(new InfoButton(ActionType.Attack, 238, 38, 32, 32, buttonsImage));
            _actionButtons.Add(new InfoButton(ActionType.Run, 278, 38, 32, 32, buttonsImage));
            _actionButtons.Add(new InfoButton(ActionType.Heal, 238, 78, 32, 32, buttonsImage));
            _actionButtons.Add(new InfoButton(ActionType.Burn, 278, 78, 32, 32, buttonsImage));
            _actionButtons.Add(new InfoButton(ActionType.Unlock, 238, 118, 32, 32, buttonsImage));
            _actionButtons.Add(new InfoButton(ActionType.Light, 278, 118, 32, 32, buttonsImage));
            _actionButtons.Add(new InfoButton(ActionType.Freeze, 238, 158, 32, 32, buttonsImage));
            _actionButtons.Add(new InfoButton(ActionType.Reflect, 278, 158, 32, 32, buttonsImage));

            foreach (var button in _actionButtons)
            {
                button.SetSelectedImage(buttonSelectedImage, 40, 40, 4, 4);
                button.Selected = button == _actionButtons.First();
                button.Enabled = true;
            }

            Console.WriteLine("CombatManager initialized");
        }

        public void StartCombat(string enemyId, string uniqueFlag = "")
        {
            Reset();
            LoadEnemy(enemyId, uniqueFlag);
            UpdateSpells();

            _uniqueFlag = uniqueFlag;
            _phase = CombatPhase.Intro;
            _timer = 15;
        }

        public void UpdateSpells()
        {
            foreach (var button in _actionButtons)
            {
                button.Selected = false;

                if (button.Action.Equals(ActionType.Attack) ||
                    button.Action.Equals(ActionType.Run))
                {
                    button.Enabled = true;
                    continue;
                }

                if (_avatar.KnowsSpell(Enum.GetName(typeof(ActionType), button.Action)))
                {
                    button.Enabled = true;
                    continue;
                }

                button.Enabled = false;
            }

            _actionButtons.Where(x => x.Enabled).First().Selected = true;
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
            _exploreManager.RenderWorld();

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
                    RenderActionButtons();
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

            // Action selection
            if (_windowManager.KeyPressed(InputKey.KEY_DOWN) ||
                _windowManager.KeyPressed(InputKey.KEY_RIGHT))
            {
                var buttons = _actionButtons
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

            if (_windowManager.KeyPressed(InputKey.KEY_UP) ||
                _windowManager.KeyPressed(InputKey.KEY_LEFT))
            {
                var buttons = _actionButtons
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
            if (_windowManager.KeyPressed(InputKey.KEY_A))
            {
                usedAction = true;
                var buttonSelected = _actionButtons.Where(x => x.Selected).First();

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
            if (_windowManager.KeyPressed(InputKey.KEY_B))
            {
                usedAction = true;
                DoRunAction();
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
                    _soundManager.PlaySound(SoundFX.Coin);
                    GiveReward();
                    return;
                }

                if (_runSuccess)
                {
                    Reset();
                    _gameStateManager.StartExplore();

                    return;
                }

                // Enemy attack
                AttackResult attackResult = _enemy.Attack(_avatar.Armor.Defence + _avatar.Defence);

                _defenceAction = attackResult.Action;
                _defenceResult = attackResult.Result;
                _soundManager.PlaySound(attackResult.Sound);
                _heroHurt = attackResult.IsHeroDamaged;

                _avatar.AddHP(-attackResult.DamageToHeroHP);
                _avatar.AddMP(-attackResult.DamageToHeroMP);
                _enemy.AddHP(-attackResult.DamageToEnemyHP);

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
                _exploreManager.SetWorldRenderOffset(_randGen.Next(8) - 4, _randGen.Next(8) - 4);
            }

            if (_timer == 15)
            {
                _exploreManager.SetWorldRenderOffset(0, 0);
            }

            if (_timer <= 0)
            {
                // Check Defeat
                if (_avatar.HP <= 0)
                {
                    _phase = CombatPhase.Defeat;
                    _soundManager.PlaySound(SoundFX.Defeat);
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
                _gameStateManager.StartExplore();
            }
        }

        private void UpdateDefeat()
        {
            if (_windowManager.KeyPressed(InputKey.KEY_A) ||
                _windowManager.KeyPressed(InputKey.KEY_B))
            {
                _avatar.Respawn();
                _ = _exploreManager.Save();
                _exploreManager.LoadWorld(_avatar.World);
                _gameStateManager.StartExplore();
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

        private void RenderActionButtons()
        {
            foreach (var button in _actionButtons)
            {
                button.Render();
            }
        }

        private void GiveReward()
        {
            var goldReward = _enemy.GoldReward();
            _avatar.AddGold(goldReward);
            _avatar.PushCampaignFlag(_uniqueFlag);
            _rewardResult = string.Format("{0} Gold!", goldReward);
            _rewardGoldAmount = goldReward;
        }

        private void LoadEnemy(string enemyId, string uniqueFlag = "")
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

                if (uniqueFlag.Equals("9-dead-walkways@death-speaker"))
                {
                    _enemy = JsonConvert.DeserializeObject<EnemyBoss>(jsonData, new StringEnumConverter());
                    (_enemy as EnemyBoss).InitBoneShield(_resourceManager.LoadImage("Data/images/enemies/bone_shield.png"));
                }
                else
                {
                    _enemy = JsonConvert.DeserializeObject<Enemy>(jsonData, new StringEnumConverter());
                }

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

        private void DoAttackAction()
        {
            _offenceAction = "Attack!";

            // Boss bone shield
            if (_enemy is EnemyBoss &&
                (_enemy as EnemyBoss).BoneShieldActive)
            {
                _offenceResult = "Absorbed!";
                _soundManager.PlaySound(SoundFX.Blocked);
                return;
            }

            // Check Miss
            if (_randGen.Next(100) < 20)
            {
                _offenceResult = "Miss!";
                _soundManager.PlaySound(SoundFX.Miss);
                return;
            }

            // Hit
            var attackDamage = _randGen.Next(_avatar.Weapon.AttackDispersion() + 1) + _avatar.Weapon.AttackMin + _avatar.Attack;

            // Critical Hit
            if (_randGen.Next(100) < 10)
            {
                attackDamage += _avatar.Weapon.AttackMax;
                _offenceAction = "Critical!";
                _soundManager.PlaySound(SoundFX.Critical);
            }
            else
            {
                _soundManager.PlaySound(SoundFX.Attack);
            }

            _enemyHurt = true;
            _enemy.AddHP(-attackDamage);
            _offenceResult = string.Format("{0} damage", attackDamage);
        }

        private void DoRunAction()
        {
            _offenceAction = "Run!";
            _soundManager.PlaySound(SoundFX.Run);

            if (_randGen.Next(100) < 66)
            {
                _runSuccess = true;
                _offenceResult = string.Empty;
                return;
            }

            _offenceResult = "Blocked!";
        }

        private void DoHealAction()
        {
            if (_avatar.MP <= 0 ||
                _avatar.HP >= _avatar.MaxHP)
            {
                return;
            }

            int healAmount = _randGen.Next((int)(_avatar.MaxHP * 0.5)) + (int)(_avatar.MaxHP * 0.5);
            _avatar.AddHP(healAmount);
            _avatar.AddMP(-1);

            _offenceAction = "Heal!";
            _offenceResult = string.Format("+{0} HP", healAmount);
            _soundManager.PlaySound(SoundFX.Heal);
        }

        private void DoBurnAction()
        {
            if (_avatar.MP <= 0 ||
                !_avatar.KnowsSpell("Burn"))
            {
                return;
            }

            var attackDamage = _randGen.Next(_avatar.Weapon.AttackDispersion() + 1) + _avatar.Weapon.AttackMin + _avatar.Attack;

            // Against Undead burn does 2x crit
            if (_enemy.Category.Equals(EnemyCategory.Undead))
            {
                attackDamage += _avatar.Weapon.AttackMax;
                attackDamage += _avatar.Weapon.AttackMax;
            }

            // Against most creatures burn does 1x crit
            // Against demons burn does regular weapon damage
            if (!_enemy.Category.Equals(EnemyCategory.Demon))
            {
                attackDamage += _avatar.Weapon.AttackMax;
            }

            // Burn boneshield of the boss
            if (_enemy is EnemyBoss)
            {
                (_enemy as EnemyBoss).BurnBoneShield();
            }

            _avatar.AddMP(-1);

            _enemyHurt = true;
            _enemy.AddHP(-attackDamage);
            _offenceAction = "Burn!";
            _offenceResult = string.Format("{0} damage", attackDamage);
            _soundManager.PlaySound(SoundFX.Fire);
        }

        private void DoUnlockAction()
        {
            if (_avatar.MP <= 0 ||
                !_avatar.KnowsSpell("Unlock") ||
                !_enemy.Category.Equals(EnemyCategory.Automaton))
            {
                return;
            }

            var spell = _avatar.GetSpell("Unlock");
            var attackDamage = _randGen.Next(_avatar.Weapon.AttackDispersion() + 1) + _avatar.Weapon.AttackMin + _avatar.Attack;

            // Unlock can only be cast against Automatons
            // so apply the full damage
            attackDamage += _avatar.Weapon.AttackMax;
            attackDamage += _avatar.Weapon.AttackMax;

            _avatar.AddMP(-1);

            _enemyHurt = true;
            _enemy.AddHP(-attackDamage);
            _offenceAction = "Unlock!";
            _offenceResult = string.Format("{0} damage", attackDamage);
            _soundManager.PlaySound(SoundFX.Unlock);
        }
    }
}
