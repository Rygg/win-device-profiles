using DeviceProfiles.Domain.ValueObjects;

namespace DeviceProfiles.Application.Common.Interfaces;

/// <summary>
/// Interface for HotKey triggers.
/// </summary>
public interface IHotKeyTrigger
{
    /// <summary>
    /// Register a KeyCombination that can be listened to by this implementation.
    /// </summary>
    /// <param name="hotKey">Key combination to register to the listener.</param>
    /// <param name="ct">CancellationToken for the operation.</param>
    /// <exception cref="InvalidOperationException">HotKey could not be registered.</exception>
    Task RegisterHotKeyAsync(HotKeyCombination hotKey, CancellationToken ct);
    /// <summary>
    /// Waits until a registered HotKey is pressed and returns the pressed key combination.
    /// </summary>
    /// <param name="ct">CancellationToken for the operation.</param>
    /// <returns>The registered key combination.</returns>
    Task<HotKeyCombination> GetHotKeyPressAsync(CancellationToken ct);
}