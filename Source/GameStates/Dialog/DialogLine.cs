namespace DuskProject.Source.GameStates.Dialog;

using DuskProject.Source.Enums;

public record DialogLine
{
    public DialogType Type { get; init; }

    public string Value { get; init; } = string.Empty;

    public int Cost { get; init; }

    public string MessageFirst { get; init; } = string.Empty;

    public string MessageSecond { get; init; } = string.Empty;
}
