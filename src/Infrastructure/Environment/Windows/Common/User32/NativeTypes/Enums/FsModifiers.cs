﻿namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

/// <summary>
/// The keys that must be pressed in combination with the key specified by the uVirtKey parameter in order to generate the WM_HOTKEY message.
/// The fsModifiers parameter can be a combination of the following values.
/// </summary>
[Flags]
internal enum FsModifiers : uint
{
    /// <summary>
    /// Either ALT key must be held down.
    /// </summary>
    MOD_ALT = 0x0001,
    /// <summary>
    /// Either CTRL key must be held down.
    /// </summary>
    MOD_CONTROL = 0x0002,
    /// <summary>
    /// Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications. Windows Vista:  This flag is not supported.
    /// </summary>
    MOD_NOREPEAT = 0x4000,
    /// <summary>
    /// Either SHIFT key must be held down.
    /// </summary>
    MOD_SHIFT = 0x0004,
    /// <summary>
    /// Either WINDOWS key was held down. These keys are labeled with the Windows logo. Keyboard shortcuts that involve the WINDOWS key are reserved for use by the operating system. 
    /// </summary>
    MOD_WIN = 0x0008,
}