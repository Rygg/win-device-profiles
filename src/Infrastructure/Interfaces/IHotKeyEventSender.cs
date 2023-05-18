using Infrastructure.Interfaces.Models;

namespace Infrastructure.Interfaces;

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