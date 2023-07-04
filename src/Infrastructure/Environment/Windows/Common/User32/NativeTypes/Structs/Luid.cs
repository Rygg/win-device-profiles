using System.Globalization;
using System.Runtime.InteropServices;

namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// Describes a local identifier for an adapter.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct LUID
{
    /// <summary>
    /// Specifies a DWORD that contains the unsigned lower numbers of the id.
    /// </summary>
    internal uint LowPart;
    /// <summary>
    /// Specifies a LONG that contains the signed high numbers of the id.
    /// </summary>
    internal int HighPart;
    /// <summary>
    /// Value.
    /// </summary>
    internal long Value => (long)HighPart << 32 | LowPart;
    /// <summary>
    /// Returns the value as a string.
    /// </summary>
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}