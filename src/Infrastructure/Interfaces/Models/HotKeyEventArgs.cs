using DeviceProfiles.Domain.Enums;

namespace DeviceProfiles.Infrastructure.Interfaces.Models;

/// <summary>
/// Event Arguments for the key press events.
/// </summary>
public sealed class HotKeyEventArgs : EventArgs
{   
    /// <summary>
    /// Pressed Modifier keys.
    /// </summary>
    public SupportedKeyModifiers Modifiers { get; init; }
    /// <summary>
    /// Pressed key.
    /// </summary>
    public SupportedKeys Key { get; init; }
    /// <summary>
    /// Hotkey registration Id.
    /// </summary>
    public int RegistrationId { get; init; }

    /// <summary>
    /// Print this EventArgs as a string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{nameof(HotKeyEventArgs)}| Modifiers: {Modifiers}, Key: {Key}, RegistrationId: {RegistrationId}";
    }
}