using System.Runtime.InteropServices;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
/// <summary>
/// The DISPLAYCONFIG_VIDEO_SIGNAL_INFO structure contains information about the video signal for a display.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
{
    /// <summary>
    /// The pixel clock rate.
    /// </summary>
    internal ulong pixelRate;
    /// <summary>
    /// A DISPLAYCONFIG_RATIONAL structure that represents horizontal sync.
    /// </summary>
    internal DISPLAYCONFIG_RATIONAL hSyncFreq;
    /// <summary>
    /// A DISPLAYCONFIG_RATIONAL structure that represents vertical sync.
    /// </summary>
    internal DISPLAYCONFIG_RATIONAL vSyncFreq;
    /// <summary>
    /// A DISPLAYCONFIG_2DREGION structure that specifies the width and height (in pixels) of the active portion of the video 
    /// </summary>
    internal DISPLAYCONFIG_2DREGION activeSize;
    /// <summary>
    /// A DISPLAYCONFIG_2DREGION structure that specifies the width and height (in pixels) of the entire video signal.
    /// </summary>
    internal DISPLAYCONFIG_2DREGION totalSize;
    /// <summary>
    /// The video standard (if any) that defines the video signal. For a list of possible values, see the D3DKMDT_VIDEO_SIGNAL_STANDARD enumerated type.
    /// </summary>
    internal D3DKMDT_VIDEO_SIGNAL_STANDARD videoStandard;
    /// <summary>
    /// The scan-line ordering (for example, progressive or interlaced) of the video signal. For a list of possible values, see the DISPLAYCONFIG_SCANLINE_ORDERING enumerated type.
    /// </summary>
    internal DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
}
