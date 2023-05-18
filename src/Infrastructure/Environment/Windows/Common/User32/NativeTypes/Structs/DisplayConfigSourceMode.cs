using System.Runtime.InteropServices;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// The DISPLAYCONFIG_SOURCE_MODE structure represents a point or an offset in a two-dimensional space.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_SOURCE_MODE
{
    /// <summary>
    /// The width in pixels of the source mode.
    /// </summary>
    internal uint width;
    /// <summary>
    /// The height in pixels of the source mode.
    /// </summary>
    internal uint height;
    /// <summary>
    /// A value from the DISPLAYCONFIG_PIXELFORMAT enumeration that specifies the pixel format of the source mode.
    /// </summary>
    internal DISPLAYCONFIG_PIXELFORMAT pixelFormat;
    /// <summary>
    /// A POINTL structure that specifies the position in the desktop coordinate space of the upper-left corner of this source surface. The source surface that is located at (0, 0) is always the primary source surface.
    /// </summary>
    internal POINTL position;
}
