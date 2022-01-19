using System.Runtime.InteropServices;

namespace Win32NativeMethods.User32.Display.NativeTypes
{
    /// <summary>
    /// he DISPLAYCONFIG_MODE_INFO structure contains either source mode or target mode information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_MODE_INFO
    {
        /// <summary>
        /// A value that indicates whether the DISPLAYCONFIG_MODE_INFO structure represents source or target mode information. If infoType is DISPLAYCONFIG_MODE_INFO_TYPE_TARGET, the targetMode parameter value contains a valid DISPLAYCONFIG_TARGET_MODE structure describing the specified target. If infoType is DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE, the sourceMode parameter value contains a valid DISPLAYCONFIG_SOURCE_MODE structure describing the specified source.
        /// </summary>
        public DISPLAYCONFIG_MODE_INFO_TYPE infoType;
        /// <summary>
        /// The source or target identifier on the specified adapter that this path relates to.
        /// </summary>
        public uint id;
        /// <summary>
        /// The identifier of the adapter that this source or target mode information relates to.
        /// </summary>
        public LUID adapterId;
        /// <summary>
        /// Info based on infoType
        /// </summary>
        public DISPLAYCONFIG_MODE_INFO_union info;
    }

    /// <summary>
    /// The DISPLAYCONFIG_MODE_INFO_TYPE enumeration specifies that the information that is contained within the DISPLAYCONFIG_MODE_INFO structure is either source or target mode.
    /// </summary>
    public enum DISPLAYCONFIG_MODE_INFO_TYPE
    {
        /// <summary>
        /// Indicates that the DISPLAYCONFIG_MODE_INFO structure contains source mode information.
        /// </summary>
        DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
        /// <summary>
        /// DISPLAYCONFIG_MODE_INFO_TYPE
        /// </summary>
        DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
        /// <summary>
        /// Indicates that the DISPLAYCONFIG_MODE_INFO structure contains a valid DISPLAYCONFIG_DESKTOP_IMAGE_INFO structure. Supported starting in Windows 10.
        /// </summary>
        DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE = 3,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DISPLAYCONFIG_MODE_INFO_union
    {
        [FieldOffset(0)]
        public DISPLAYCONFIG_TARGET_MODE targetMode;

        [FieldOffset(0)]
        public DISPLAYCONFIG_SOURCE_MODE sourceMode;

        [FieldOffset(0)]
        public DISPLAYCONFIG_DESKTOP_IMAGE_INFO desktopImageInfo;
    }

    /// <summary>
    /// The DISPLAYCONFIG_TARGET_MODE structure describes a display path target mode.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_TARGET_MODE
    {
        /// <summary>
        /// A DISPLAYCONFIG_VIDEO_SIGNAL_INFO structure that contains a detailed description of the current target mode.
        /// </summary>
        public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
    }

    /// <summary>
    /// The DISPLAYCONFIG_VIDEO_SIGNAL_INFO structure contains information about the video signal for a display.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
    {
        /// <summary>
        /// The pixel clock rate.
        /// </summary>
        public ulong pixelRate;
        /// <summary>
        /// A DISPLAYCONFIG_RATIONAL structure that represents horizontal sync.
        /// </summary>
        public DISPLAYCONFIG_RATIONAL hSyncFreq;
        /// <summary>
        /// A DISPLAYCONFIG_RATIONAL structure that represents vertical sync.
        /// </summary>
        public DISPLAYCONFIG_RATIONAL vSyncFreq;
        /// <summary>
        /// A DISPLAYCONFIG_2DREGION structure that specifies the width and height (in pixels) of the active portion of the video 
        /// </summary>
        public DISPLAYCONFIG_2DREGION activeSize;
        /// <summary>
        /// A DISPLAYCONFIG_2DREGION structure that specifies the width and height (in pixels) of the entire video signal.
        /// </summary>
        public DISPLAYCONFIG_2DREGION totalSize;
        /// <summary>
        /// The video standard (if any) that defines the video signal. For a list of possible values, see the D3DKMDT_VIDEO_SIGNAL_STANDARD enumerated type.
        /// </summary>
        public D3DKMDT_VIDEO_SIGNAL_STANDARD videoStandard;
        /// <summary>
        /// The scan-line ordering (for example, progressive or interlaced) of the video signal. For a list of possible values, see the DISPLAYCONFIG_SCANLINE_ORDERING enumerated type.
        /// </summary>
        public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
    }


    /// <summary>
    /// The DISPLAYCONFIG_SOURCE_MODE structure represents a point or an offset in a two-dimensional space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_SOURCE_MODE
    {
        /// <summary>
        /// The width in pixels of the source mode.
        /// </summary>
        public uint width;
        /// <summary>
        /// The height in pixels of the source mode.
        /// </summary>
        public uint height;
        /// <summary>
        /// A value from the DISPLAYCONFIG_PIXELFORMAT enumeration that specifies the pixel format of the source mode.
        /// </summary>
        public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
        /// <summary>
        /// A POINTL structure that specifies the position in the desktop coordinate space of the upper-left corner of this source surface. The source surface that is located at (0, 0) is always the primary source surface.
        /// </summary>
        public POINTL position;
    }

    /// <summary>
    /// The DISPLAYCONFIG_DESKTOP_IMAGE_INFO structure contains information about the image displayed on the desktop.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_DESKTOP_IMAGE_INFO
    {
        /// <summary>
        /// A POINTL structure that specifies the size of the VidPn source surface that is being displayed on the monitor.
        /// </summary>
        public POINTL PathSourceSize;
        /// <summary>
        /// A RECTL structure that defines where the desktop image will be positioned within path source. Region must be completely inside the bounds of the path source size.
        /// </summary>
        public RECTL DesktopImageRegion;
        /// <summary>
        /// A RECTL structure that defines which part of the desktop image for this clone group will be displayed on this path. This currently must be set to the desktop size.
        /// </summary>
        public RECTL DesktopImageClip;
    }
}
