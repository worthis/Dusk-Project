namespace DuskProject.Source.GameStates.Dialog;

public record Store
{
    public string Name { get; init; } = string.Empty;

    public string Music { get; init; } = string.Empty;

    public int BackgroundImage { get; init; } = 0;

    public DialogLine[] Lines { get; init; } = Array.Empty<DialogLine>();
}
