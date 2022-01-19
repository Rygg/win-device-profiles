namespace Win32NativeMethods.User32.Display.NativeTypes
{
    /// <summary>
    /// The type of information to retrieve. The value for the Flags parameter must use one of the following values. 
    /// </summary>
    public enum QDC
    {
        /// <summary>
        /// The caller requests the table sizes to hold all the possible path combinations.
        /// </summary>
        QDC_ALL_PATHS = 0x00000001,
        /// <summary>
        /// The caller requests the table sizes to hold only active paths.
        /// </summary>
        QDC_ONLY_ACTIVE_PATHS = 0x00000002,
        /// <summary>
        /// The caller requests the table sizes to hold the active paths as defined in the persistence database for the currently connected monitors.
        /// </summary>
        QDC_DATABASE_CURRENT = 0x00000004,
        /// <summary>
        /// This flag should be bitwise OR'ed with other flags to indicate that the caller is aware of virtual mode support. Supported starting in Windows 10
        /// </summary>
        QDC_VIRTUAL_MODE_AWARE = 0x00000010,
        /// <summary>
        /// This flag should be bitwise OR'ed with QDC_ONLY_ACTIVE_PATHS to indicate that the caller would like to include head-mounted displays (HMDs) in the list of active paths. See Remarks for more information. 
        /// Supported starting in Windows 10 1703 Creators Update.
        /// </summary>
        QDC_INCLUDE_HMD = 0x00000020,
        /// <summary>
        /// This flag should be bitwise OR'ed with other flags to indicate that the caller is aware of virtual refresh rate support. Supported starting in Windows 11.
        /// </summary>
        QDC_VIRTUAL_REFRESH_RATE_AWARE = 0x00000040,
    }
}
