using System.Runtime.InteropServices;

namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
/// <summary>
/// The DISPLAYCONFIG_TARGET_MODE structure describes a display path target mode.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_TARGET_MODE
{
    /// <summary>
    /// A DISPLAYCONFIG_VIDEO_SIGNAL_INFO structure that contains a detailed description of the current target mode.
    /// </summary>
    internal DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
}
