using System.Runtime.InteropServices;

namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// Describes a local identifier for an adapter.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LUID
    { 
        /// <summary>
        /// Specifies a DWORD that contains the unsigned lower numbers of the id.
        /// </summary>
        public uint LowPart;
        /// <summary>
        /// Specifies a LONG that contains the signed high numbers of the id.
        /// </summary>
        public int HighPart;
        /// <summary>
        /// Value.
        /// </summary>
        public long Value => ((long)HighPart << 32) | LowPart;
        public override string ToString() => Value.ToString();
    }
}
