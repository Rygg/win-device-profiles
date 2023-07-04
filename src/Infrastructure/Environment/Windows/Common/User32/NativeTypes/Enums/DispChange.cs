namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

/// <summary>
/// The ChangeDisplaySettingsEx function returns one of the following values.
/// </summary>
internal enum DISP_CHANGE : int
{
    /// <summary>
    /// The settings change was successful. 
    /// </summary>
    Successful = 0,
    /// <summary>
    /// The computer must be restarted for the graphics mode to work. 
    /// </summary>
    Restart = 1,
    /// <summary>
    /// The display driver failed the specified graphics mode.
    /// </summary>
    Failed = -1,
    /// <summary>
    /// The graphics mode is not supported. 
    /// </summary>
    BadMode = -2,
    /// <summary>
    /// Unable to write settings to the registry. 
    /// </summary>
    NotUpdated = -3,
    /// <summary>
    /// An invalid set of flags was passed in. 
    /// </summary>
    BadFlags = -4,
    /// <summary>
    /// An invalid parameter was passed in. This can include an invalid flag or combination of flags. 
    /// </summary>
    BadParam = -5,
    /// <summary>
    /// The settings change was unsuccessful because the system is DualView capable.
    /// </summary>
    BadDualView = -6
}