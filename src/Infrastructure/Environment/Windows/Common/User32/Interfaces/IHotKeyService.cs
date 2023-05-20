using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace Infrastructure.Environment.Windows.Common.User32.Interfaces;

internal interface IHotKeyService
{
    bool RegisterHotKeyToHandle(IntPtr hWnd, int id, FsModifiers fsModifiers, uint vlc);
    bool UnregisterHotKeyFromHandle(IntPtr hWnd, int id);
}