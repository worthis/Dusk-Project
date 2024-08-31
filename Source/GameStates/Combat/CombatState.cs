namespace DuskProject.Source.GameStates.Combat;

using DuskProject.Source.Creatures;
using DuskProject.Source.Enums;
using DuskProject.Source.GameStates.Combat.Phases;
using DuskProject.Source.Interfaces;
using DuskProject.Source.Resources;
using DuskProject.Source.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class CombatState : IGameState
{
    private GameStateManager _gameStateManager;
    private WindowManager _windowManager;
    private ResourceManager _resourceManager;
    private SoundManager _soundManager;
    private TextManager _textManager;
    private WorldManager _worldManager;
    private Avatar _avatar;

    private Enemy _enemy;
    private string _uniqueFlag = string.Empty;

    private List<InfoButton> _actionButtons = new List<InfoButton>();

    private string _rewardResult = string.Empty;
    private int _rewardGoldAmount = 0;

    public CombatState(
        GameStateManager gameStateManager,
        WindowManager windowManager,
        ResourceManager resourceManager,
        SoundManager soundManager,
        TextManager textManager,
        WorldManager worldManager,
        Avatar avatar)
    {
        _gameStateManager = gameStateManager;
        _windowManager = windowManager;
        _resourceManager = resourceManager;
        _soundManager = soundManager;
        _textManager = textManager;
        _worldManager = worldManager;
        _avatar = avatar;

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

        Init();

        Console.WriteLine("Combat State created");
    }

    public GameStateManager GameStateManager { get => _gameStateManager; }

    public WindowManager WindowManager { get => _windowManager; }

    public SoundManager SoundManager { get => _soundManager; }

    public TextManager TextManager { get => _textManager; }

    public WorldManager WorldManager { get => _worldManager; }

    public Avatar Avatar { get => _avatar; }

    public CombatPhaseBase Phase { get; set; }

    public IList<InfoButton> ActionButtons { get => _actionButtons; }

    public int Timer { get; set; }

    public Enemy Enemy { get => _enemy; }

    public bool EnemyHurt { get; set; } = false;

    public bool HeroHurt { get; set; } = false;

    public bool RunSuccess { get; set; } = false;

    public (string Action, string Result) Offence { get; set; } = (string.Empty, string.Empty);

    public (string Action, string Result) Defence { get; set; } = (string.Empty, string.Empty);

    public (int Amount, string Result) Reward { get; set; } = (0, string.Empty);

    public void Render()
    {
        _worldManager.RenderBackground(_avatar.Facing);
        _worldManager.RenderWorld(_avatar.X, _avatar.Y, _avatar.Facing);

        Phase.Render();

        _textManager.Render(_enemy.Name, 160, 4, TextJustify.JUSTIFY_CENTER);
    }

    public void Update()
    {
        _textManager.Color = _avatar.IsBadlyHurt() ? TextColor.Red : TextColor.Default;

        Phase.Update();
    }

    public void RenderHeroStats(bool showGold = false)
    {
        _textManager.Render(string.Format("HP {0}/{1}", _avatar.HP, _avatar.MaxHP), 4, 200);
        _textManager.Render(string.Format("MP {0}/{1}", _avatar.MP, _avatar.MaxMP), 4, 220);

        if (showGold)
        {
            _textManager.Render(string.Format("{0} Gold", _avatar.Gold), 316, 220, TextJustify.JUSTIFY_RIGHT);
        }
    }

    public void RenderOffenceLog()
    {
        if (string.IsNullOrEmpty(Offence.Action))
        {
            return;
        }

        _textManager.Render("You:", 4, 40);
        _textManager.Render(Offence.Action, 4, 60);
        _textManager.Render(Offence.Result, 4, 80);
    }

    public void RenderDefenceLog()
    {
        if (string.IsNullOrEmpty(Defence.Action))
        {
            return;
        }

        _textManager.Render("Enemy:", 4, 120);
        _textManager.Render(Defence.Action, 4, 140);
        _textManager.Render(Defence.Result, 4, 160);
    }

    public void RenderActionButtons()
    {
        foreach (var button in _actionButtons)
        {
            button.Render();
        }
    }

    private void Init()
    {
        LoadEnemy(_gameStateManager.Enemy.Id, _gameStateManager.Enemy.UniqueFlag);
        UpdateSpells();

        Phase = new CombatIntroPhase(this);
        Timer = 15;
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

    private void UpdateSpells()
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
}
