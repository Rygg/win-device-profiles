using Infrastructure.Common.Interfaces.Args;

namespace Infrastructure.Common.Interfaces;

/// <summary>
/// Interface for key events.
/// </summary>
public interface IHotKeyEventSender
{
    /// <summary>
    /// Any key was pressed.
    /// </summary>
    event Action<HotKeyEventArgs> OnRegisteredHotKeyPressed;
}