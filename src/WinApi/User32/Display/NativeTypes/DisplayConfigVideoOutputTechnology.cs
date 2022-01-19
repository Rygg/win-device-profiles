namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY enumeration specifies the target's connector type.
    /// </summary>
    public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY
    {
        /// <summary>
        /// Indicates a connector that is not one of the types that is indicated by the following enumerators in this enumeration.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = -1,
        /// <summary>
        /// Indicates an HD15 (VGA) connector.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
        /// <summary>
        /// Indicates an S-video connector.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
        /// <summary>
        /// Indicates a composite video connector group.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
        /// <summary>
        /// Indicates a component video connector group.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
        /// <summary>
        /// Indicates a Digital Video Interface (DVI) connector.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
        /// <summary>
        /// Indicates a High-Definition Multimedia Interface (HDMI) connector.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
        /// <summary>
        /// Indicates a Low Voltage Differential Swing (LVDS) connector.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
        /// <summary>
        /// Indicates a Japanese D connector.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
        /// <summary>
        /// Indicates an SDI connector.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
        /// <summary>
        /// Indicates an external display port, which is a display port that connects externally to a display device.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
        /// <summary>
        /// Indicates an embedded display port that connects internally to a display device.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
        /// <summary>
        /// Indicates an external Unified Display Interface (UDI), which is a UDI that connects externally to a display device.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
        /// <summary>
        /// Indicates an embedded UDI that connects internally to a display device.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
        /// <summary>
        /// Indicates a dongle cable that supports standard definition television (SDTV).
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
        /// <summary>
        /// Indicates that the VidPN target is a Miracast wireless display device. Supported starting in Windows 8.1.
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INDIRECT_WIRED = 16,
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INDIRECT_VIRTUAL = 17,
        /// <summary>
        /// Indicates that the video output device connects internally to a display device (for example, the internal connection in a laptop computer).
        /// </summary>
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = unchecked((int)0x80000000),
    }
}
