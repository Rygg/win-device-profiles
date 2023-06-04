using Domain.Enums;

namespace Domain.ValueObjects;

/// <summary>
/// Model for HotKey combinations.
/// </summary>
public sealed record HotKeyCombination
{
    /// <summary>
    /// Modifier keys required for the hotkey to trigger.
    /// </summary>
    public SupportedKeyModifiers Modifiers { get; init; }
    /// <summary>
    /// HotKey.
    /// </summary>
    public SupportedKeys Key { get; init; }
}