namespace Win32NativeMethods.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_ROTATION enumeration specifies the clockwise rotation of the display.
    /// </summary>
    public enum DISPLAYCONFIG_ROTATION
    {
        /// <summary>
        /// Indicates that rotation is 0 degrees—landscape mode.
        /// </summary>
        DISPLAYCONFIG_ROTATION_IDENTITY = 1,
        /// <summary>
        /// Indicates that rotation is 90 degrees clockwise—portrait mode.
        /// </summary>
        DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
        /// <summary>
        /// Indicates that rotation is 180 degrees clockwise—inverted landscape mode.
        /// </summary>
        DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
        /// <summary>
        /// Indicates that rotation is 270 degrees clockwise—inverted portrait mode.
        /// </summary>
        DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
    }
}
