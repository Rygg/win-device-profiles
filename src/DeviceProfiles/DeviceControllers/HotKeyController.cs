using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeviceProfiles.Classes;
using DeviceProfiles.Enums;
using NLog;
using Win32NativeMethods.User32.Keyboard;
using Win32NativeMethods.User32.Keyboard.NativeTypes;

namespace DeviceProfiles.DeviceControllers
{
    /// <summary>
    /// Controller for handling the hot key functionality.
    /// </summary>
    internal sealed class HotKeyController : IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The actual windows responsible for handling the hot keys.
        /// </summary>
        private readonly HotKeyWindow _window;
        /// <summary>
        /// List tracking the currently registered hot keys.
        /// </summary>
        private readonly List<HotKeyRegistration> _registeredHotKeys;
        /// <summary>
        /// Variable tracking the registered hotkey identifiers.
        /// </summary>
        private int _currentHotKeyRegistrationId;
        /// <summary>
        /// Default constructor for HotKeyController.
        /// </summary>
        internal HotKeyController()
        {
            _window = new HotKeyWindow();
            _currentHotKeyRegistrationId = 0; // Start hotkey registration from 0.
            _registeredHotKeys = new List<HotKeyRegistration>();
        }

        /// <summary>
        /// Register a hot key.
        /// </summary>
        /// <param name="profileId">Link to the profile.</param>
        /// <param name="hotKey">Requested key configuration.</param>
        /// <exception cref="InvalidOperationException">Hot key could not be registered.</exception>
        internal void RegisterHotKey(int profileId, DeviceProfileHotKey hotKey)
        {
            var fsModifiers = (FsModifiers)hotKey.Modifiers;
            var key = (uint)hotKey.Key;

            var alreadyRegistered = _registeredHotKeys.Any(r => r.Key == key && (int)r.Modifiers == (int)fsModifiers); // Check if key combination is already registered.
            if(alreadyRegistered)
            {
                Log.Warn("Key combination is already registered. Returning");
                return;
            }

            _currentHotKeyRegistrationId++; // Increment the operation counter. Registration starts from 1 -->
            Log.Debug($"Registering global hot key for the application. HotKeyId: {_currentHotKeyRegistrationId}. Modifiers: {hotKey.Modifiers}, Key: {hotKey.Key}");

            if (!NativeMethods.RegisterHotKey(_window.Handle, _currentHotKeyRegistrationId, fsModifiers, key)) // Register the hot key.
            {
                throw new InvalidOperationException("Hot key could not be registered.");
            }
            _registeredHotKeys.Add(new HotKeyRegistration(profileId, _currentHotKeyRegistrationId, fsModifiers, key)); // Add to registered hot keys.
            Log.Info("HotKey registered.");
        }

        /// <summary>
        /// Task subscribes for the HotKeyPressed-events and returns the pressed hotkeys and modifiers.
        /// </summary>
        /// <param name="ct">CancellationToken</param>
        /// <returns></returns>
        internal Task<HotKeyRegistration> GetHotKeyPressAsync(CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<HotKeyRegistration>(); // Create tcs.

            // local function as action for the event.
            void RequestAction(KeyPressedEventArgs args) {
                
                var registrationId = args.RegistrationId;
                var registration = _registeredHotKeys.FirstOrDefault(r => r.HotKeyRegistrationId == registrationId); // Get registration.
                if (registration == null)
                {
                    Log.Error("HotKeyRegistration not found for the received event.");
                    tcs.TrySetException(new IndexOutOfRangeException("HotKeyRegistration not found for the received event."));
                }
                else
                {
                    tcs.TrySetResult(registration);
                }
            }

            _window.HotKeyPressed += RequestAction; // Subscribe to event.

            var ctRegistration = ct.Register(() => tcs.TrySetCanceled()); // Register the CT cancellation to cancel the Tcs.

            return tcs.Task.ContinueWith(async t =>
            {
                await ctRegistration.DisposeAsync(); // Dispose the ct registration when completed.
                _window.HotKeyPressed -= RequestAction; // Remove event listener.
                return await t;
            }, CancellationToken.None).Unwrap(); // Return unwrapped task.
        }

        /// <summary>
        /// Dispose the controller class.
        /// </summary>
        public void Dispose()
        {
            Log.Trace("Disposing..");
            // Unregister all hotkeys registered. This can be done by looping from currentHotKeyRegistrationId to 1.
            for(var i = _currentHotKeyRegistrationId; i > 0; i--)
            {
                Log.Debug($"Unregistering hot key registration: RegistrationId: {_currentHotKeyRegistrationId}");
                NativeMethods.UnregisterHotKey(_window.Handle, i); // Unregister.
            }
            _window.Dispose(); // Dispose the inner window.
            Log.Trace("Disposed.");
        }



        /// <summary>
        /// Class for handling the hot key functionality. Inherited from<see cref="NativeWindow"/> to provide handler to native keyboard events.
        /// </summary>
        private sealed class HotKeyWindow : NativeWindow, IDisposable
        {
            private static readonly Logger Log = LogManager.GetCurrentClassLogger();

            /// <summary>
            /// Public event for parent to listen to the requested hotkeys. 
            /// </summary>
            public event Action<KeyPressedEventArgs>? HotKeyPressed;

            /// <summary>
            /// Message identifier for Windows HotKey Messages.
            /// </summary>
            private const int WM_HOTKEY = 0x0312; 

            public HotKeyWindow()
            {
                CreateHandle(new CreateParams()); // Create handle for this window.
            }
            /// <summary>
            /// Override WndProc to get the notifications to the parent in events.
            /// </summary>
            /// <param name="m">The operating system message</param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m); // Run base function.

                if (m.Msg == WM_HOTKEY) // Check for hotkey operating system message. Ignore others.
                {
                    // Get the key values from the LParameters:
                    // Key is stored in bits 4 to 7:
                    var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF); //  Get the values of the bits using shifting 4 bits to the right and using a bitwise and-operation.
                    var modifiers = (EKeyModifiers)((int)m.LParam & 0xFFFF); // Modifiers are stored in bits from 0 to 3. Use bitwise and-operation to get the value.
                    var registrationId = (int)m.WParam; // The identifier of the hot key that generated the message. If the message was generated by a system-defined hot key, this parameter will be one of the following values.
                    // Invoke the event in case parent is listening.
                    HotKeyPressed?.Invoke(new KeyPressedEventArgs(modifiers, key, registrationId));
                }
            }
            /// <summary>
            /// Dispose the window.
            /// </summary>
            public void Dispose()
            {
                Log.Trace("Disposing..");
                DestroyHandle(); // Destroy the handle.
                Log.Trace("Disposed.");
            }
        }
    }
}
