namespace Infrastructure.Windows.Native.User32.Display.NativeTypes
{
    /// <summary>
    /// A bitwise OR of flag values that indicates the status of the source. The following values are supported:
    /// </summary>
    [Flags]
    public enum DISPLAYCONFIG_SOURCE_FLAGS
    {
        /// <summary>
        /// This source is in use by at least one active path.
        /// </summary>
        DISPLAYCONFIG_SOURCE_IN_USE = 0x00000001,
    }
}
