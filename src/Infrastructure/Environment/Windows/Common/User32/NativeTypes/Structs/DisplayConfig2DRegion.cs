using System.Runtime.InteropServices;

namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// The DISPLAYCONFIG_2DREGION structure represents a point or an offset in a two-dimensional space.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_2DREGION
{
    /// <summary>
    /// The horizontal component of the point or offset.
    /// </summary>
    public uint cx;
    /// <summary>
    /// The vertical component of the point or offset.
    /// </summary>
    public uint cy;
}