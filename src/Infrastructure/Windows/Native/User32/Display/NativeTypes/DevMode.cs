using System.Runtime.InteropServices;

namespace Infrastructure.Windows.Native.User32.Display.NativeTypes
{
    /// <summary>
    /// The DEVMODE data structure contains information about the initialization and environment of a printer or a display device.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    public struct DEVMODE
    {
        public const int CCHDEVICENAME = 32;
        public const int CCHFORMNAME = 32;
        /// <summary>
        /// A zero-terminated character array that specifies the "friendly" name of the printer or display; for example, "PCL/HP LaserJet" in the case of PCL/HP LaserJet. 
        /// This string is unique among device drivers. Note that this name may be truncated to fit in the dmDeviceName array.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        [FieldOffset(0)]
        public string dmDeviceName;
        /// <summary>
        /// The version number of the initialization data specification on which the structure is based. To ensure the correct version is used for any operating system, use DM_SPECVERSION.
        /// </summary>
        [FieldOffset(32)]
        public short dmSpecVersion;
        /// <summary>
        /// The driver version number assigned by the driver developer.
        /// </summary>
        [FieldOffset(34)]
        public short dmDriverVersion;
        /// <summary>
        /// Specifies the size, in bytes, of the DEVMODE structure, not including any private driver-specific data that might follow the structure's public members. 
        /// Set this member to sizeof (DEVMODE) to indicate the version of the DEVMODE structure being used.
        /// </summary>
        [FieldOffset(36)]
        public short dmSize;
        /// <summary>
        /// Contains the number of bytes of private driver-data that follow this structure. If a device driver does not use device-specific information, set this member to zero.
        /// </summary>
        [FieldOffset(38)]
        public short dmDriverExtra;
        /// <summary>
        /// Specifies whether certain members of the DEVMODE structure have been initialized. If a member is initialized, its corresponding bit is set, otherwise the bit is clear. 
        /// A driver supports only those DEVMODE members that are appropriate for the printer or display technology.
        /// </summary>
        [FieldOffset(40)]
        public uint dmFields;

        [FieldOffset(44)]
        short dmOrientation;
        [FieldOffset(46)]
        short dmPaperSize;
        [FieldOffset(48)]
        short dmPaperLength;
        [FieldOffset(50)]
        short dmPaperWidth;
        [FieldOffset(52)]
        short dmScale;
        [FieldOffset(54)]
        short dmCopies;
        [FieldOffset(56)]
        short dmDefaultSource;
        [FieldOffset(58)]
        short dmPrintQuality;
        /// <summary>
        /// For display devices only, a POINTL structure that indicates the positional coordinates of the display device in reference to the desktop area. The primary display device is always located at coordinates (0,0).
        /// </summary>
        [FieldOffset(44)]
        public POINTL dmPosition;
        /// <summary>
        /// For display devices only, the orientation at which images should be presented. If DM_DISPLAYORIENTATION is not set, this member must be zero. 
        /// If DM_DISPLAYORIENTATION is set, this member must be one of the following values
        /// <list type="bullet">
        /// <item>0 - DMDO_DEFAULT 	The display orientation is the natural orientation of the display device; it should be used as the default.</item>
        /// <item>1 - DMDO_90 The display orientation is rotated 90 degrees(measured clockwise) from DMDO_DEFAULT.</item>
        /// <item>2 - DMDO_180    The display orientation is rotated 180 degrees (measured clockwise) from DMDO_DEFAULT.</item>
        /// <item>3 - DMDO_270 The display orientation is rotated 270 degrees (measured clockwise) from DMDO_DEFAULT.</item>
        /// </list>
        /// </summary>
        [FieldOffset(52)]
        public int dmDisplayOrientation;
        /// <summary>
        /// For fixed-resolution display devices only, how the display presents a low-resolution mode on a higher-resolution display. 
        /// For example, if a display device's resolution is fixed at 1024 x 768 pixels but its mode is set to 640 x 480 pixels, 
        /// the device can either display a 640 x 480 image somewhere in the interior of the 1024 x 768 screen space or stretch the 640 x 480 image to fill the larger screen space. 
        /// If DM_DISPLAYFIXEDOUTPUT is not set, this member must be zero. If DM_DISPLAYFIXEDOUTPUT is set, this member must be one of the following values.
        /// </summary>
        /// <list type="bullet">
        /// <item>0 - DMDFO_DEFAULT The display's default setting.</item>
        /// <item>1 - DMDFO_CENTER 	The low-resolution image is centered in the larger screen space.</item>
        /// <item>2 - DMDFO_STRETCH 	The low-resolution image is stretched to fill the larger screen space.</item>
        /// </list>
        [FieldOffset(56)]
        public int dmDisplayFixedOutput;

        [FieldOffset(60)]
        public short dmColor; // See note below!
        [FieldOffset(62)]
        public short dmDuplex; // See note below!
        [FieldOffset(64)]
        public short dmYResolution;
        [FieldOffset(66)]
        public short dmTTOption;
        [FieldOffset(68)]
        public short dmCollate; // See note below!
        [FieldOffset(72)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        [FieldOffset(102)]
        public short dmLogPixels;
        [FieldOffset(104)]
        public int dmBitsPerPel;
        [FieldOffset(108)]
        public int dmPelsWidth;
        [FieldOffset(112)]
        public int dmPelsHeight;
        [FieldOffset(116)]
        public int dmDisplayFlags;
        [FieldOffset(116)]
        public int dmNup;
        [FieldOffset(120)]
        public int dmDisplayFrequency;
    }

}
