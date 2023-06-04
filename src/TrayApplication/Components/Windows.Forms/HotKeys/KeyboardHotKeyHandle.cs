﻿using Domain.Enums;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;
using Microsoft.Extensions.Logging;
using TrayApplication.Extensions;

namespace TrayApplication.Components.Windows.Forms.HotKeys;

/// <summary>
/// Class for handling the hot key functionality. Inherited from<see cref="NativeWindow"/> to provide handler to native keyboard events.
/// </summary>
public sealed class KeyboardHotKeyHandle : NativeWindow, IDisposable, IWindowsHotKeyEventSender
{
    /// <summary>
    /// Injected logger.
    /// </summary>
    private readonly ILogger<KeyboardHotKeyHandle> _logger;
    /// <summary>
    /// Public event to listen for key presses implementing the required interface.
    /// </summary>
    public event EventHandler<HotKeyEventArgs>? OnRegisteredHotKeyPressed;
    /// <summary>
    /// Message identifier for Windows HotKey Messages.
    /// </summary>
    private const int WmHotkey = 0x0312;

    public KeyboardHotKeyHandle(ILogger<KeyboardHotKeyHandle> logger)
    {
        _logger = logger;
        CreateHandle(new CreateParams()); // Create handle for this window.
    }
    /// <summary>
    /// Override WndProc to get the notifications for the events.
    /// </summary>
    /// <param name="m">The operating system message</param>
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m); // Run base function.

        if (m.Msg == WmHotkey) // Check for hotkey operating system message. Ignore others.
        {
            // Get the key values from the LParameters:
            // Key is stored in bits 4 to 7:
            var key = (SupportedKeys)((int)m.LParam >> 16 & 0xFFFF); //  Get the values of the bits using shifting 4 bits to the right and using a bitwise and-operation.
            var modifiers = (SupportedKeyModifiers)((int)m.LParam & 0xFFFF); // Modifiers are stored in bits from 0 to 3. Use bitwise and-operation to get the value.
            var registrationId = (int)m.WParam; // The identifier of the hot key that generated the message. If the message was generated by a system-defined hot key, this parameter will be one of the following values.
            _logger.HotKeyEventReceived(modifiers, key, registrationId);
            OnRegisteredHotKeyPressed?.Invoke(this, new HotKeyEventArgs // Invoke the event for listeners.
            {
                Modifiers = modifiers,
                Key = key,
                RegistrationId = registrationId
            });
        }
    }
    /// <summary>
    /// Dispose the window.
    /// </summary>
    public void Dispose()
    {
        _logger.StartingDispose();
        DestroyHandle(); // Destroy the handle.
        _logger.DisposingCompleted();
    }
}