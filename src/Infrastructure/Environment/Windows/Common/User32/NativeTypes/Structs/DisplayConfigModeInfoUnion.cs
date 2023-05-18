using System.Runtime.InteropServices;

namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
[StructLayout(LayoutKind.Explicit)]
internal struct DISPLAYCONFIG_MODE_INFO_union
{
    [FieldOffset(0)]
    internal DISPLAYCONFIG_TARGET_MODE targetMode;

    [FieldOffset(0)]
    internal DISPLAYCONFIG_SOURCE_MODE sourceMode;

    [FieldOffset(0)]
    internal DISPLAYCONFIG_DESKTOP_IMAGE_INFO desktopImageInfo;
}