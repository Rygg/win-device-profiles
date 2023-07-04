using DeviceProfiles.Domain.Entities;

namespace DeviceProfiles.TrayApplication.Components.Interfaces;

/// <summary>
/// Interface for classes providing functionality for TrayIcons.
/// </summary>
public interface ITrayIconProvider
{
    /// <summary>
    /// Get the TrayIcon object for the application context.
    /// </summary>
    NotifyIcon TrayIcon { get; }

    /// <summary>
    /// Provide an optional callback that will be called when the tray icon is closed.
    /// </summary>
    /// <param name="closingCallback">Callback action to execute when the form is closing.</param>
    void SetOnCloseCallback(Action closingCallback);

    /// <summary>
    /// Update tray icon contents. TODO : Is this actually required in the interface? Probably not. Should handle internally
    /// </summary>
    void UpdateTrayIconContents(DeviceProfile[] profiles);

}