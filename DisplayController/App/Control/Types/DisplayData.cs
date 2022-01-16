using WinApi.User32.Display.NativeTypes;

namespace DisplayController.App.Control.Types
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
