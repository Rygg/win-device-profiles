using System.Runtime.InteropServices;

namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// The DISPLAYCONFIG_RATIONAL structure describes a fractional value that represents vertical and horizontal frequencies of a video mode (that is, vertical sync and horizontal sync).
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_RATIONAL
{
    /// <summary>
    /// The numerator of the frequency fraction.
    /// </summary>
    internal uint Numerator;
    /// <summary>
    /// The denominator of the frequency fraction.
    /// </summary>
    internal uint Denominator;

    public override string ToString() => Numerator + " / " + Denominator;
}