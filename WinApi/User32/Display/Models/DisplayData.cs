using WinApi.User32.Display.NativeTypes;

namespace WinApi.User32.Display.Models
{
    internal class DisplayData
    {
        public DISPLAY_DEVICE Device;
        public DEVMODE DeviceMode;
        public DisplayData(DISPLAY_DEVICE device, DEVMODE deviceMode)
        {
            Device = device;
            DeviceMode = deviceMode;
        }
    }
}
