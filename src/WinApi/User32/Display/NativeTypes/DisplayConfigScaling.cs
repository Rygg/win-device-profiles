namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_SCALING enumeration specifies the scaling transformation applied to content displayed on a video present network (VidPN) present path.
    /// </summary>
    public enum DISPLAYCONFIG_SCALING
    {
        /// <summary>
        /// Indicates the identity transformation; the source content is presented with no change. This transformation is available only if the path's source mode has the same spatial resolution as the path's target mode.
        /// </summary>
        DISPLAYCONFIG_SCALING_IDENTITY = 1,
        /// <summary>
        /// Indicates the centering transformation; the source content is presented unscaled, centered with respect to the spatial resolution of the target mode.
        /// </summary>
        DISPLAYCONFIG_SCALING_CENTERED = 2,
        /// <summary>
        /// Indicates the content is scaled to fit the path's target.
        /// </summary>
        DISPLAYCONFIG_SCALING_STRETCHED = 3,
        /// <summary>
        /// Indicates the aspect-ratio centering transformation.
        /// </summary>
        DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
        /// <summary>
        /// Indicates that the caller requests a custom scaling that the caller cannot describe with any of the other DISPLAYCONFIG_SCALING_XXX values. Only a hardware vendor's value-add application should use DISPLAYCONFIG_SCALING_CUSTOM, because the value-add application might require a private interface to the driver. The application can then use DISPLAYCONFIG_SCALING_CUSTOM to indicate additional context for the driver for the custom value on the specified path.
        /// </summary>
        DISPLAYCONFIG_SCALING_CUSTOM = 5,
        /// <summary>
        /// Indicates that the caller does not have any preference for the scaling. The SetDisplayConfig function will use the scaling value that was last saved in the database for the path. If such a scaling value does not exist, SetDisplayConfig will use the default scaling for the computer. For example, stretched (DISPLAYCONFIG_SCALING_STRETCHED) for tablet computers and aspect-ratio centered (DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX) for non-tablet computers.
        /// </summary>
        DISPLAYCONFIG_SCALING_PREFERRED = 128,
    }
}
