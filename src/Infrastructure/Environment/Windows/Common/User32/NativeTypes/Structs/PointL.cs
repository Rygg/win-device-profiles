using System.Runtime.InteropServices;

namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// The POINTL structure contains the coordinates of a point.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct POINTL
{
    /// <summary>
    /// The horizontal (x) coordinate of the point.
    /// </summary>
    internal int x;
    /// <summary>
    /// The vertical (y) coordinate of the point.
    /// </summary>
    internal int y;
}