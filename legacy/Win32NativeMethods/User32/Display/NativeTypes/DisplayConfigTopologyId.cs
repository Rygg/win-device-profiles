namespace Win32NativeMethods.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_TOPOLOGY_ID enumeration specifies the type of display topology.
    /// </summary>
    public enum DISPLAYCONFIG_TOPOLOGY_ID
    {
        /// <summary>
        /// Indicates that the display topology is an internal configuration.
        /// </summary>
        DISPLAYCONFIG_TOPOLOGY_INTERNAL = 0x00000001,
        /// <summary>
        /// Indicates that the display topology is clone-view configuration.
        /// </summary>
        DISPLAYCONFIG_TOPOLOGY_CLONE = 0x00000002,
        /// <summary>
        /// Indicates that the display topology is an extended configuration.
        /// </summary>
        DISPLAYCONFIG_TOPOLOGY_EXTEND = 0x00000004,
        /// <summary>
        /// Indicates that the display topology is an external configuration.
        /// </summary>
        DISPLAYCONFIG_TOPOLOGY_EXTERNAL = 0x00000008,
    }
}
