namespace Infrastructure.Windows.Native.User32.Display.NativeTypes
{
    /// <summary>
    /// A bitwise OR of flag values that indicates the status of the target. The following values are supported:
    /// </summary>
    [Flags]
    public enum DISPLAYCONFIG_TARGET_FLAGS
    {
        /// <summary>
        /// Target is in use on an active path. 
        /// </summary>
        DISPLAYCONFIG_TARGET_IN_USE = 0x00000001,
        /// <summary>
        /// The output can be forced on this target even if a monitor is not detected. 
        /// </summary>
        DISPLAYCONFIG_TARGET_FORCIBLE = 0x00000002,
        /// <summary>
        /// Output is currently being forced in a boot-persistent manner. 
        /// </summary>
        DISPLAYCONFIG_TARGET_FORCED_AVAILABILITY_BOOT = 0x00000004,
        /// <summary>
        /// Output is currently being forced in a path-persistent manner.
        /// </summary>
        DISPLAYCONFIG_TARGET_FORCED_AVAILABILITY_PATH = 0x00000008,
        /// <summary>
        /// Output is currently being forced in a nonpersistent manner. 
        /// </summary>
        DISPLAYCONFIG_TARGET_FORCED_AVAILABILITY_SYSTEM = 0x00000010,
        /// <summary>
        /// The output is a head-mounted display (HMD). Such a path is only returned from QueryDisplayConfig using the QDC_INCLUDE_HMD flag. Supported starting in the Windows 10 Creators Update (Version 1703).
        /// </summary>
        DISPLAYCONFIG_TARGET_IS_HMD = 0x00000020,
    }
}
