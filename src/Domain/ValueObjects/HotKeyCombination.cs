using DeviceProfiles.Domain.Enums;

namespace DeviceProfiles.Domain.ValueObjects;

/// <summary>
/// Model for HotKey combinations.
/// </summary>
public sealed class HotKeyCombination
{
    /// <summary>
    /// Modifier keys required for the hotkey to trigger.
    /// </summary>
    public SupportedKeyModifiers Modifiers { get; init; }
    /// <summary>
    /// HotKey.
    /// </summary>
    public SupportedKeys Key { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is not HotKeyCombination hk)
        {
            return false;
        }

        return (int)Modifiers == (int)hk.Modifiers && (int)Key == (int)hk.Key;
    }

    public override int GetHashCode()
    {
        return Modifiers.GetHashCode() ^ Key.GetHashCode();
    }
}