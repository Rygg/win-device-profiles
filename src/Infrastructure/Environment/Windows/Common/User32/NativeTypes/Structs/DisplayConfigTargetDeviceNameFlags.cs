using System.Runtime.InteropServices;

namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
/// <summary>
/// The DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS structure contains information about a target device.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
{
    /// <summary>
    /// A member in the union that DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS contains that can hold a 32-bit value that identifies information about the device.
    /// </summary>
    internal uint value;
}
