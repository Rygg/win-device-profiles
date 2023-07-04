using DeviceProfiles.Infrastructure.Interfaces.Models;

namespace DeviceProfiles.Infrastructure.Interfaces;

/// <summary>
/// Interface for key events.
/// </summary>
public interface IHotKeyEventSender
{
    /// <summary>
    /// Any key was pressed.
    /// </summary>
    event EventHandler<HotKeyEventArgs>? OnRegisteredHotKeyPressed;
}