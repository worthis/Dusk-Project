﻿namespace DuskProject.Source.GameStates.Dialog;

using DuskProject.Source.Creatures;
using DuskProject.Source.Enums;
using DuskProject.Source.Interfaces;
using DuskProject.Source.Resources;
using DuskProject.Source.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class DialogState : IGameState
{
    private GameStateManager _gameStateManager;
    private WindowManager _windowManager;
    private ResourceManager _resourceManager;
    private SoundManager _soundManager;
    private TextManager _textManager;
    private WorldManager _worldManager;
    private ItemManager _itemManager;
    private Avatar _avatar;

    private Store _store;
    private bool _hasSellingItems = false;
    private TimedMessage _message = new TimedMessage(timeOut: 2000);
    private DialogButton[] _buttons;

    public DialogState(
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

        ImageResource buttonsImage = _resourceManager.LoadImage("Data/images/interface/dialog_buttons.png");
        ImageResource buttonSelectedImage = _resourceManager.LoadImage("Data/images/interface/select.png");

        _buttons = new DialogButton[3]
        {
            new DialogButton(8, 120, 32, 32, buttonsImage),
            new DialogButton(8, 160, 32, 32, buttonsImage),
            new DialogButton(8, 200, 32, 32, buttonsImage),
        };

        foreach (var button in _buttons)
        {
            button.SetSelectedImage(buttonSelectedImage, 40, 40, 4, 4);
        }

        LoadStore(_gameStateManager.Store);
        ResetButtons();
        UpdateButtons();

        Console.WriteLine("Dialog State created");
    }

    public void Render()
    {
        _worldManager.RenderBackground(_store.BackgroundImage);

        _textManager.Render(_store.Name, 160, 4, TextJustify.JUSTIFY_CENTER);

        RenderButtons();
        RenderTexts();

        if (_hasSellingItems)
        {
            RenderGold();
        }

        _textManager.Render(_message.Text, 160, 80, TextJustify.JUSTIFY_CENTER);
    }

    public void Update()
    {
        _message.Update();
        _textManager.Color = _avatar.IsBadlyHurt() ? TextColor.Red : TextColor.Default;

        if (_windowManager.KeyPressed(InputKey.KEY_UP) ||
            _windowManager.KeyPressed(InputKey.KEY_DOWN))
        {
            var buttons = _buttons
                .Where(x =>
                {
                    return x.Enabled && !x.Action.Equals(DialogButtonAction.None);
                })
                .ToList();

            if (_windowManager.KeyPressed(InputKey.KEY_DOWN))
            {
                buttons.Reverse();
            }

            foreach (var button in buttons)
            {
                if (button.Selected &&
                    !button.Equals(buttons.First()))
                {
                    button.Selected = false;
                    buttons[buttons.IndexOf(button) - 1].Selected = true;
                }
            }
        }

        if (_windowManager.KeyPressed(InputKey.KEY_A))
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                if (!_buttons[i].Selected ||
                    !_buttons[i].Enabled)
                {
                    continue;
                }

                if (_buttons[i].Action.Equals(DialogButtonAction.Exit))
                {
                    _gameStateManager.StartExplore();
                    return;
                }

                if (_buttons[i].Action.Equals(DialogButtonAction.Buy))
                {
                    switch (_store.Lines[i].Type)
                    {
                        case DialogType.Weapon:
                        case DialogType.Armor:
                            if (_itemManager.GetItem(_store.Lines[i].Value, out var item))
                            {
                                if (_avatar.Gold < item.Gold)
                                {
                                    _message.Start("Your gold isn't enough!");
                                    return;
                                }

                                _avatar.AddGold(-item.Gold);

                                _avatar.EquipItem(item);
                                _message.Start(string.Format("Bought {0}", item.Name));
                            }

                            break;

                        case DialogType.Spell:
                            if (_itemManager.GetItem(_store.Lines[i].Value, out var spell))
                            {
                                if (_avatar.Gold < spell.Gold)
                                {
                                    _message.Start("Your gold isn't enough!");
                                    return;
                                }

                                _avatar.AddGold(-spell.Gold);

                                _avatar.LearnSpell(spell);
                                _message.Start(string.Format("Learned {0}", spell.Name));
                            }

                            break;

                        case DialogType.Room:
                            if (_avatar.Gold < _store.Lines[i].Cost)
                            {
                                _message.Start("Your gold isn't enough!");
                                return;
                            }

                            _avatar.AddGold(-_store.Lines[i].Cost);

                            _avatar.Sleep();
                            _message.Start("You have rested");

                            break;
                    }

                    UpdateButtons();
                    _soundManager.PlaySound(SoundFX.Coin);

                    return;
                }
            }
        }

        if (_windowManager.KeyPressed(InputKey.KEY_B))
        {
            _gameStateManager.StartExplore();
            return;
        }
    }

    private void RenderButtons()
    {
        foreach (var button in _buttons)
        {
            button.Render();
        }
    }

    private void RenderTexts()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (string.IsNullOrEmpty(_buttons[i].TextFirst) &&
                string.IsNullOrEmpty(_buttons[i].TextSecond))
            {
                continue;
            }

            if (string.IsNullOrEmpty(_buttons[i].TextSecond))
            {
                _textManager.Render(
                    _buttons[i].TextFirst,
                    _buttons[i].X + (_buttons[i].Action.Equals(DialogButtonAction.None) ? 4 : 44),
                    _buttons[i].Y + 8);

                continue;
            }

            _textManager.Render(
                    _buttons[i].TextFirst,
                    _buttons[i].X + (_buttons[i].Action.Equals(DialogButtonAction.None) ? 4 : 44),
                    _buttons[i].Y - 2);
            _textManager.Render(
                    _buttons[i].TextSecond,
                    _buttons[i].X + (_buttons[i].Action.Equals(DialogButtonAction.None) ? 4 : 44),
                    _buttons[i].Y + 18);
        }
    }

    private void RenderGold()
    {
        _textManager.Render(string.Format("{0} Gold", _avatar.Gold), 316, 220, TextJustify.JUSTIFY_RIGHT);
    }

    private void UpdateButtons()
    {
        _hasSellingItems = false;

        int linesCount = _store.Lines.Length > _buttons.Length ? _buttons.Length : _store.Lines.Length;
        for (int i = 0; i < linesCount; i++)
        {
            _buttons[i].Enabled = false;

            switch (_store.Lines[i].Type)
            {
                case DialogType.Weapon:
                case DialogType.Armor:
                    if (_itemManager.GetItem(_store.Lines[i].Value, out var item))
                    {
                        _hasSellingItems = true;
                        _buttons[i].Action = DialogButtonAction.Buy;
                        _buttons[i].TextFirst = string.Format("Buy {0}", item.Name);

                        if (_avatar.HasItem(item))
                        {
                            _buttons[i].TextSecond = "(You own this)";
                        }
                        else if (!_avatar.IsBetterItem(item))
                        {
                            _buttons[i].TextSecond = "(Yours is better)";
                        }
                        else
                        {
                            _buttons[i].TextSecond = string.Format("for {0} Gold", item.Gold);
                            _buttons[i].Enabled = true;
                        }
                    }

                    break;

                case DialogType.Spell:
                    if (_itemManager.GetItem(_store.Lines[i].Value, out var spell))
                    {
                        _hasSellingItems = true;
                        _buttons[i].Action = DialogButtonAction.Buy;
                        _buttons[i].TextFirst = string.Format("Learn {0}", spell.Name);

                        if (_avatar.KnowsSpell(spell.Name))
                        {
                            _buttons[i].TextSecond = "(You know this)";
                        }
                        else if (spell.Level > _avatar.SpellBookLevel + 1)
                        {
                            _buttons[i].TextSecond = "(Too advanced)";
                        }
                        else
                        {
                            _buttons[i].TextSecond = string.Format("for {0} Gold", spell.Gold);
                            _buttons[i].Enabled = true;
                        }
                    }

                    break;

                case DialogType.Room:
                    _hasSellingItems = true;
                    _buttons[i].Action = DialogButtonAction.Buy;
                    _buttons[i].TextFirst = "Rent a room for rest";

                    if (_avatar.IsRested())
                    {
                        _buttons[i].TextSecond = "(You are well rested)";
                    }
                    else
                    {
                        _buttons[i].TextSecond = string.Format("for {0} Gold", _store.Lines[i].Cost);
                        _buttons[i].Enabled = true;
                    }

                    break;

                case DialogType.Message:
                    _buttons[i].TextFirst = _store.Lines[i].MessageFirst;
                    _buttons[i].TextSecond = _store.Lines[i].MessageSecond;

                    break;
            }

            if (_buttons[i].Selected &&
                !_buttons[i].Enabled)
            {
                _buttons[_buttons.Length - 1].Selected = true;
            }
        }
    }

    private void ResetButtons()
    {
        _hasSellingItems = false;

        for (int i = 0; i < _buttons.Length; i++)
        {
            // Last button for Exit
            int lastButton = _buttons.Length - 1;
            _buttons[i].Action = (i == lastButton) ? DialogButtonAction.Exit : DialogButtonAction.None;
            _buttons[i].Selected = i == lastButton;
            _buttons[i].TextFirst = i == lastButton ? "Exit" : string.Empty;
            _buttons[i].TextSecond = string.Empty;
            _buttons[i].Enabled = i == lastButton;
        }
    }

    private void LoadStore(string storeName)
    {
        // Loading store from file
        string fileName = string.Format("Data/stores/{0}.json", storeName);

        if (!File.Exists(fileName))
        {
            Console.WriteLine("Error: Unable to load store {0} from {1}", storeName, fileName);
            return;
        }

        using (StreamReader streamReader = new(fileName))
        {
            string jsonData = streamReader.ReadToEnd();
            streamReader.Close();

            _store = JsonConvert.DeserializeObject<Store>(jsonData, new StringEnumConverter());
        }

        if (_store is null)
        {
            Console.WriteLine("Error: Unable to load broken store file {0}", fileName);
            return;
        }

        _soundManager.PlayMusic(_store.Music);

        Console.WriteLine("Store {0} loaded from {1}", storeName, fileName);
    }
}
