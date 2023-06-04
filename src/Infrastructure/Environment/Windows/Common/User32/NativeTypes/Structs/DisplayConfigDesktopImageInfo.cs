using System.Runtime.InteropServices;

namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
/// <summary>
/// The DISPLAYCONFIG_DESKTOP_IMAGE_INFO structure contains information about the image displayed on the desktop.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_DESKTOP_IMAGE_INFO
{
    /// <summary>
    /// A POINTL structure that specifies the size of the VidPn source surface that is being displayed on the monitor.
    /// </summary>
    internal POINTL PathSourceSize;
    /// <summary>
    /// A RECTL structure that defines where the desktop image will be positioned within path source. Region must be completely inside the bounds of the path source size.
    /// </summary>
    internal RECTL DesktopImageRegion;
    /// <summary>
    /// A RECTL structure that defines which part of the desktop image for this clone group will be displayed on this path. This currently must be set to the desktop size.
    /// </summary>
    internal RECTL DesktopImageClip;
}
