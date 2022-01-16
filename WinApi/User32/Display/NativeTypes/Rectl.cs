using System.Runtime.InteropServices;

namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// The RECTL structure defines a rectangle by the coordinates of its upper-left and lower-right corners
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECTL
    {
        /// <summary>
        /// Specifies the x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int left;
        /// <summary>
        /// Specifies the y-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int top;
        /// <summary>
        /// Specifies the x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int right;
        /// <summary>
        /// Specifies the y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int bottom;
    }

}
