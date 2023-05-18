using System.Runtime.InteropServices;

namespace Infrastructure.Windows.Native.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_2DREGION structure represents a point or an offset in a two-dimensional space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_2DREGION
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
}
