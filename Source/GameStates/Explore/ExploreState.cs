namespace DuskProject.Source.GameStates.Explore;

using DuskProject.Source.Creatures;
using DuskProject.Source.Dialog;
using DuskProject.Source.Enums;
using DuskProject.Source.Interfaces;
using DuskProject.Source.UI;
using DuskProject.Source.World;

public class ExploreState : IGameState
{
    private const int _encounterIncrement = 5;
    private const int _encounterChanceMax = 30;

    private GameStateManager _gameStateManager;
    private WindowManager _windowManager;
    private ResourceManager _resourceManager;
    private SoundManager _soundManager;
    private TextManager _textManager;
    private WorldManager _worldManager;
    private ItemManager _itemManager;
    private Avatar _avatar;

    private TimedMessage _message = new TimedMessage(timeOut: 2000);

    private Random _randGen;
    private int _encounterChance = 0;

    public ExploreState(
        GameStateManager gameStateManager,
        WindowManager windowManager,
        ResourceManager resourceManager,
        SoundManager soundManager,
        TextManager textManager,
        WorldManager worldManager,
        ItemManager itemManager,
        Avatar avatar)
    {
        _gameStateManager = gameStateManager;
        _windowManager = windowManager;
        _resourceManager = resourceManager;
        _soundManager = soundManager;
        _textManager = textManager;
        _worldManager = worldManager;
        _itemManager = itemManager;
        _avatar = avatar;

        // Miyoo Mini Plus does not have RTC time
        _randGen = new Random(Environment.TickCount);

        Console.WriteLine("Explore State created");
    }

    public void Render()
    {
        _worldManager.RenderBackground(_avatar.Facing);
        _worldManager.RenderWorld(_avatar.X, _avatar.Y, _avatar.Facing);

        // UI
        // Compass
        RenderCompass();

        // Minimap
        _worldManager.Minimap.Render();
        _worldManager.Minimap.RenderHero(_avatar.X, _avatar.Y, _avatar.Facing);

        // Messages
        _textManager.Render(_message.Text, 160, 200, TextJustify.JUSTIFY_CENTER);
    }

    public void Update()
    {
        // Update Input
        // Info Screen
        if (_windowManager.KeyPressed(InputKey.KEY_SELECT))
        {
            _soundManager.PlaySound(SoundFX.Click);
            _gameStateManager.ShowInGameMenu();

            return;
        }

        // Main Menu
        if (_windowManager.KeyPressed(InputKey.KEY_START))
        {
            _soundManager.StopMusic();
            _soundManager.PlaySound(SoundFX.Click);
            _ = _gameStateManager.SaveGame();
            _gameStateManager.ShowMainMenu();

            return;
        }

        // Minimap
        if (_windowManager.KeyPressed(InputKey.KEY_X))
        {
            _worldManager.Minimap.Enabled = !_worldManager.Minimap.Enabled;
        }

        // Avatar Movement
        _avatar.Moved = false;

        if (_windowManager.KeyPressed(InputKey.KEY_UP))
        {
            _avatar.GetFrontTilePos(out int posX, out int posY);
            Tile tile = _worldManager.GetTile(posX, posY);

            if (tile is not null &&
                tile.Walkable)
            {
                _avatar.X = posX;
                _avatar.Y = posY;
                _avatar.Moved = true;
            }
            else
            {
                _soundManager.PlaySound(SoundFX.Blocked);
            }
        }

        if (_windowManager.KeyPressed(InputKey.KEY_DOWN))
        {
            _avatar.GetBehindTilePos(out int posX, out int posY);
            Tile tile = _worldManager.GetTile(posX, posY);

            if (tile is not null &&
                tile.Walkable)
            {
                _avatar.X = posX;
                _avatar.Y = posY;
                _avatar.Moved = true;
            }
            else
            {
                _soundManager.PlaySound(SoundFX.Blocked);
            }
        }

        if (_windowManager.KeyPressed(InputKey.KEY_LEFT))
        {
            _avatar.TurnLeft();
        }

        if (_windowManager.KeyPressed(InputKey.KEY_RIGHT))
        {
            _avatar.TurnRight();
        }

        _message.Update();
        _textManager.Color = _avatar.IsBadlyHurt() ? TextColor.Red : TextColor.Default;

        if (_avatar.Moved)
        {
            // Check exit portals
            if (_worldManager.CheckPortals(_avatar.X, _avatar.Y, out WorldPortal worldPortal))
            {
                _avatar.X = worldPortal.DestX;
                _avatar.Y = worldPortal.DestY;
                _avatar.World = worldPortal.Destination;
                _worldManager.LoadWorld(worldPortal.Destination);
                _worldManager.InitScriptedEvents(_avatar.HasCampaignFlag);
                _message.Start(_worldManager.WorldName);

                return;
            }

            // Check store entrance
            if (_worldManager.CheckStores(_avatar.X, _avatar.Y, out StorePortal storePortal))
            {
                _avatar.X = storePortal.DestX;
                _avatar.Y = storePortal.DestY;
                _gameStateManager.StartDialog(storePortal.Store);

                return;
            }

            // Special scripts
            // Message Point
            if (_worldManager.CheckMessagePoints(_avatar.X, _avatar.Y, out MessagePoint messagePoint))
            {
                if (!_avatar.HasCampaignFlag(messagePoint.UniqueId))
                {
                    _message.Start(messagePoint.Message);
                    _soundManager.PlaySound(messagePoint.Sound);
                    _avatar.PushCampaignFlag(messagePoint.UniqueId);
                }
            }

            // Rest Points
            if (_worldManager.CheckRestPoints(_avatar.X, _avatar.Y, out RestPoint restPoint))
            {
                _avatar.Sleep();
                _ = _gameStateManager.SaveGame();
                _message.Start(restPoint.Message);
                _soundManager.PlaySound(restPoint.Sound);

                return;
            }

            // Chests
            if (_worldManager.CheckChests(_avatar.X, _avatar.Y, out ChestPoint chestPoint))
            {
                // todo: render treasure icon
                if (!_avatar.HasCampaignFlag(chestPoint.UniqueId))
                {
                    switch (chestPoint.RewardType)
                    {
                        case ChestRewardType.Gold:
                            _avatar.AddGold(chestPoint.RewardItemAmount);
                            break;

                        case ChestRewardType.Weapon:
                        case ChestRewardType.Armor:
                            if (_itemManager.GetItem(chestPoint.RewardItemId, out Item item))
                            {
                                if (_avatar.IsBetterItem(item))
                                {
                                    _avatar.EquipItem(item);
                                }
                            }

                            break;

                        case ChestRewardType.Spell:
                            if (_itemManager.GetItem(chestPoint.RewardItemId, out Item spell))
                            {
                                _avatar.LearnSpell(spell);
                            }

                            break;

                        case ChestRewardType.PowerUp:
                            if (chestPoint.RewardItemId.Equals("Magic Sapphire (MP Up)"))
                            {
                                _avatar.MaxMP += 2;
                                _avatar.AddMP(2);
                            }

                            if (chestPoint.RewardItemId.Equals("Magic Emerald (HP Up)"))
                            {
                                _avatar.MaxHP += 5;
                                _avatar.AddHP(5);
                            }

                            if (chestPoint.RewardItemId.Equals("Magic Ruby (Atk Up)"))
                            {
                                _avatar.Attack++;
                            }

                            if (chestPoint.RewardItemId.Equals("Magic Diamond (Def Up)"))
                            {
                                _avatar.Defence++;
                            }

                            break;
                    }

                    _worldManager.SetTileId(chestPoint.X, chestPoint.Y, chestPoint.OpenedTileId);
                    _avatar.PushCampaignFlag(chestPoint.UniqueId);
                    _soundManager.PlaySound(chestPoint.Sound);

                    if (chestPoint.RewardItemAmount > 1)
                    {
                        _message.Start(string.Format("Found {0} {1}!", chestPoint.RewardItemAmount, chestPoint.RewardItemId));
                    }
                    else
                    {
                        _message.Start(string.Format("Found {0}!", chestPoint.RewardItemId));
                    }

                    return;
                }
            }

            // Scripted Enemies
            // todo: world update on boss kill
            if (_worldManager.CheckScriptedEnemies(_avatar.X, _avatar.Y, out ScriptedEnemy scriptedEnemy))
            {
                if (!_avatar.HasCampaignFlag(scriptedEnemy.UniqueId))
                {
                    _encounterChance = 0;
                    _message.Clear();
                    _gameStateManager.StartCombat(scriptedEnemy.EnemyId, scriptedEnemy.UniqueId);

                    return;
                }
            }

            // Encounters
            if (_worldManager.Enemies is not null &&
                _worldManager.Enemies.Count > 0)
            {
                if (_randGen.Next(100) < _encounterChance)
                {
                    _encounterChance = 0;
                    _message.Clear();
                    var enemyIndex = _randGen.Next(_worldManager.Enemies.Count);
                    _gameStateManager.StartCombat(_worldManager.Enemies[enemyIndex]);

                    return;
                }

                _encounterChance += _encounterIncrement;
                if (_encounterChance > _encounterChanceMax)
                {
                    _encounterChance = _encounterChanceMax;
                }
            }
        }
    }

    private void RenderCompass()
    {
        switch (_avatar.Facing)
        {
            case AvatarFacing.North:
                _textManager.Render("NORTH", 160, 4, TextJustify.JUSTIFY_CENTER);
                break;

            case AvatarFacing.East:
                _textManager.Render("EAST", 160, 4, TextJustify.JUSTIFY_CENTER);
                break;

            case AvatarFacing.West:
                _textManager.Render("WEST", 160, 4, TextJustify.JUSTIFY_CENTER);
                break;

            case AvatarFacing.South:
                _textManager.Render("SOUTH", 160, 4, TextJustify.JUSTIFY_CENTER);
                break;
        }
    }
}
