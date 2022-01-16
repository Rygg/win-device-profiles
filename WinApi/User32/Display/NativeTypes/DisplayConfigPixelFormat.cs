namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_PIXELFORMAT enumeration specifies pixel format in various bits per pixel (BPP) values.
    /// </summary>
    public enum DISPLAYCONFIG_PIXELFORMAT
    {
        /// <summary>
        /// Indicates 8 BPP format.
        /// </summary>
        DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
        /// <summary>
        /// Indicates 16 BPP format.
        /// </summary>
        DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
        /// <summary>
        /// Indicates 24 BPP format.
        /// </summary>
        DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
        /// <summary>
        /// Indicates 32 BPP format.
        /// </summary>
        DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
        /// <summary>
        /// Indicates that the current display is not an 8, 16, 24, or 32 BPP GDI desktop mode. For example, a call to the QueryDisplayConfig function returns DISPLAYCONFIG_PIXELFORMAT_NONGDI if a DirectX application previously set the desktop to A2R10G10B10 format. A call to the SetDisplayConfig function fails if any pixel formats for active paths are set to DISPLAYCONFIG_PIXELFORMAT_NONGDI.
        /// </summary>
        DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,
    }
}
