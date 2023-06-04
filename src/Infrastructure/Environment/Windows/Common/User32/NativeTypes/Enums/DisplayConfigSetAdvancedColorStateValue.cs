namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;
/// <summary>
/// Enumeration of possible values for DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE.value.
/// </summary>
internal enum DisplayConfigSetAdvancedColorStateValue : uint
{
    /// <summary>
    /// Disables advanced color state.
    /// </summary>
    Disable = 0x0,
    /// <summary>
    /// Enables advanced color state.
    /// </summary>
    Enable = 0x1,
}
