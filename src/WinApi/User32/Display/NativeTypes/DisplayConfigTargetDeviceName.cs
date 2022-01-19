using System.Runtime.InteropServices;

namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_TARGET_DEVICE_NAME structure contains information about the target
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
    {
        /// <summary>
        /// A DISPLAYCONFIG_DEVICE_INFO_HEADER structure that contains information about the request for the target device name. The caller should set the type member of DISPLAYCONFIG_DEVICE_INFO_HEADER to DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME and the adapterId and id members of DISPLAYCONFIG_DEVICE_INFO_HEADER to the target for which the caller wants the target device name. The caller should set the size member of DISPLAYCONFIG_DEVICE_INFO_HEADER to at least the size of the DISPLAYCONFIG_TARGET_DEVICE_NAME structure.
        /// </summary>
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        /// <summary>
        /// A DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS structure that identifies, in bit-field flags, information about the target.
        /// </summary>
        public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
        /// <summary>
        /// A value from the DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY enumeration that specifies the target's connector type.
        /// </summary>
        public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
        /// <summary>
        /// The manufacture identifier from the monitor extended display identification data (EDID). This member is set only when the edidIdsValid bit-field is set in the flags member.
        /// </summary>
        public ushort edidManufactureId;
        /// <summary>
        /// The product code from the monitor EDID. This member is set only when the edidIdsValid bit-field is set in the flags member.
        /// </summary>
        public ushort edidProductCodeId;
        /// <summary>
        /// The one-based instance number of this particular target only when the adapter has multiple targets of this type. The connector instance is a consecutive one-based number that is unique within each adapter. If this is the only target of this type on the adapter, this value is zero.
        /// </summary>
        public uint connectorInstance;
        /// <summary>
        /// A NULL-terminated WCHAR string that is the device name for the monitor. This name can be used with SetupAPI.dll to obtain the device name that is contained in the installation package.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string monitorFriendlyDeviceName;
        /// <summary>
        /// A NULL-terminated WCHAR string that is the path to the device name for the monitor. This path can be used with SetupAPI.dll to obtain the device name that is contained in the installation package.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string monitorDevicePat;
    }

    /// <summary>
    /// The DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS structure contains information about a target device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
    {
        /// <summary>
        /// A member in the union that DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS contains that can hold a 32-bit value that identifies information about the device.
        /// </summary>
        public uint value;
    }
}
