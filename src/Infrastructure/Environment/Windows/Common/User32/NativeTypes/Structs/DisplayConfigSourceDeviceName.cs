using System.Runtime.InteropServices;

namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// The DISPLAYCONFIG_SOURCE_DEVICE_NAME structure contains the GDI device name for the source or view
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct DISPLAYCONFIG_SOURCE_DEVICE_NAME
{
    /// <summary>
    /// A DISPLAYCONFIG_DEVICE_INFO_HEADER structure that contains information about the request for the source device name. The caller should set the type member of DISPLAYCONFIG_DEVICE_INFO_HEADER to DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME and the adapterId and id members of DISPLAYCONFIG_DEVICE_INFO_HEADER to the source for which the caller wants the source device name. The caller should set the size member of DISPLAYCONFIG_DEVICE_INFO_HEADER to at least the size of the DISPLAYCONFIG_SOURCE_DEVICE_NAME structure.
    /// </summary>
    internal DISPLAYCONFIG_DEVICE_INFO_HEADER header;
    /// <summary>
    /// A NULL-terminated WCHAR string that is the GDI device name for the source, or view. This name can be used in a call to EnumDisplaySettings to obtain a list of available modes for the specified source.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    internal string viewGdiDeviceName;
}