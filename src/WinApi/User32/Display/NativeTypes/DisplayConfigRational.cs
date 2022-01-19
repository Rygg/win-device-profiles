using System.Runtime.InteropServices;

namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_RATIONAL structure describes a fractional value that represents vertical and horizontal frequencies of a video mode (that is, vertical sync and horizontal sync).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_RATIONAL
    {
        /// <summary>
        /// The numerator of the frequency fraction.
        /// </summary>
        public uint Numerator;
        /// <summary>
        /// The denominator of the frequency fraction.
        /// </summary>
        public uint Denominator;

        public override string ToString() => Numerator + " / " + Denominator;
    }
}
