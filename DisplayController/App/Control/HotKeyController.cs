using DisplayController.App.Control.Types;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApi.User32.Keyboard;
using WinApi.User32.Keyboard.NativeTypes;

namespace DisplayController.App.Control
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
        /// Variable tracking the registered hotkey identifiers.
        /// </summary>
        private int _currentHotKeyRegistrationId;
        /// <summary>
        /// Default constructor for HotKeyController.
        /// </summary>
        internal HotKeyController()
        {
            _window = new HotKeyWindow();
            _currentHotKeyRegistrationId = 0; // Start hotkey registeration from 0.
        }

        /// <summary>
        /// Register a hot key.
        /// </summary>
        /// <param name="modifiers">Keyboard modifiers required for the key to trigger</param>
        /// <param name="key">Requested key</param>
        /// <exception cref="InvalidOperationException">Hot key could not be registered.</exception>
        internal void RegisterHotKey(KeyModifiers modifiers, Keys key)
        {
            _currentHotKeyRegistrationId++; // Increment the operation counter. Registration starts from 1 -->
            Log.Debug($"Registering global hot key for application. HotKeyId: {_currentHotKeyRegistrationId}. Modifiers: {modifiers}, Key: {key}");
            // register the hot key.
            if (!NativeMethods.RegisterHotKey(_window.Handle, _currentHotKeyRegistrationId, (FsModifiers)modifiers, (uint)key))
            {
                throw new InvalidOperationException("Hot key could not be registered.");
            }
            Log.Debug($"HotKey registered.");
        }

        /// <summary>
        /// Task subscribes for the HotKeyPressed-events and returns the pressed hotkeys and modifiers.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        internal Task<KeyPressedEventArgs> GetHotKeyPressAsync(CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<KeyPressedEventArgs>(); // Create tcs.

            void requestAction(KeyPressedEventArgs args) => tcs.TrySetResult(args); // Action for the event

            _window.HotKeyPressed += requestAction; // Subscribe to event.

            var ctRegistration = ct.Register(() => tcs.TrySetCanceled()); // Register the CT cancellation to cancel the Tcs.

            return tcs.Task.ContinueWith(async t =>
            {
                ctRegistration.Dispose(); // Dispose the ct registration when completed.
                _window.HotKeyPressed -= requestAction; // Remove event listener.
                return await t;
            }, CancellationToken.None).Unwrap(); // Return unwrapped task.
        }

        /// <summary>
        /// Dispose the controller class.
        /// </summary>
        public void Dispose()
        {
            Log.Trace("Disposing..");
            // Unregister all hotkeys registered. This can be done by looping from currentHotKeyRegisterationId to 1.
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
        private class HotKeyWindow : NativeWindow, IDisposable
        {
            private static readonly Logger Log = LogManager.GetCurrentClassLogger();

            /// <summary>
            /// Public event for parent to listen to the requested hotkeys. 
            /// </summary>
            public event Action<KeyPressedEventArgs> HotKeyPressed;

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
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF); //  Get the values of the bits using shifting 4 bits to the right and using a bitwise and-operation.
                    KeyModifiers modifiers = (KeyModifiers)((int)m.LParam & 0xFFFF); // Modifiers are stored in bits from 0 to 3. Use bitwise and-operation to get the value.

                    // Invoke the event in case parent is listening.
                    HotKeyPressed?.Invoke(new KeyPressedEventArgs(modifiers, key));
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
