using System.Runtime.InteropServices;

namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// The POINTL structure contains the coordinates of a point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINTL
    {
        /// <summary>
        /// The horizontal (x) coordinate of the point.
        /// </summary>
        public int x;
        /// <summary>
        /// The vertical (y) coordinate of the point.
        /// </summary>
        public int y;
    }
}
