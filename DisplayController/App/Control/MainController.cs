using DisplayController.App.Configuration;
using DisplayController.App.Control.Types;
using NLog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DisplayController.App.Control
{
    /// <summary>
    /// MainController class responsible for the application functionality.
    /// </summary>
    internal class MainController : IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// DisplayController responsible for the display operations.
        /// </summary>
        private readonly DisplayController _displays;
        /// <summary>
        /// HotKeyController responsible for the hotkey operations.
        /// </summary>
        private readonly HotKeyController _hotkeys;
        /// <summary>
        /// Stores the configuration with which the controller was started with.
        /// </summary>
        private Profile[] _displayProfiles;
        /// <summary>
        /// CancellationTokenSource controlling the background loop.
        /// </summary>
        private CancellationTokenSource _backgroundLoopCts;
        /// <summary>
        /// Lock to prevent concurrent profile switchings.
        /// </summary>
        private readonly SemaphoreSlim _profileSwitchLock = new SemaphoreSlim(1, 1);
        /// <summary>
        /// Default constructor for the MainController.
        /// </summary>
        internal MainController()
        {
            _displays = new DisplayController();
            _hotkeys = new HotKeyController();
        }

        /// <summary>
        /// Method starts the controller functionality with the configuration given as a parameter.
        /// </summary>
        /// <param name="config">Configuration for the controller.</param>
        internal void Start(AppConfiguration config)
        {
            Log.Debug("Starting MainController functionality.");
            _displayProfiles = config.Profiles; // Save profiles.
            _ = BackgroundHotKeyLoop(); // Start the background loop.
        }

        /// <summary>
        /// Background loop for listening to the hot key presses.
        /// </summary>
        /// <returns></returns>
        private async Task BackgroundHotKeyLoop()
        {
            Log.Trace($"Starting {nameof(BackgroundHotKeyLoop)}");
            _backgroundLoopCts?.Cancel();
            _backgroundLoopCts?.Dispose();
            _backgroundLoopCts = new CancellationTokenSource();
                       
            var profilesWithHotKeys = _displayProfiles.Where(p => p.HotKey != null && p.HotKey.Key != null).ToArray(); // Get profiles with hotkeys.
            foreach(var profile in profilesWithHotKeys)
            {
                Log.Debug($"Registering global hotkey for profile: {profile.Name}");
                _hotkeys.RegisterHotKey(profile.Name, profile.HotKey); // Register configured hotkey.
            }
            Log.Debug("HotKeys registered");
            while(!_backgroundLoopCts.Token.IsCancellationRequested)
            {
                var registrationEvent = await _hotkeys.GetHotKeyPressAsync(_backgroundLoopCts.Token);
                _backgroundLoopCts.Token.ThrowIfCancellationRequested(); // Throw from the loop if cancelled.
                Log.Debug($"HotKeyPress detected: Profile: {registrationEvent.ProfileName}, Modifiers: {(KeyModifiers)registrationEvent.Modifiers}, Key: {(Keys)registrationEvent.Key}");
                Log.Info($"User switched profile by hot key. Selected profile: {registrationEvent.ProfileName}");

                if(await SetDisplayProfile(registrationEvent.ProfileName, _backgroundLoopCts.Token)) // Set the requested profile.
                {
                    Log.Info("Profile changed successfully!");
                }
                else
                {
                    Log.Error("Profile could not be changed.");
                }
            }
        }

        /// <summary>
        /// Set the profile to the selected profile.
        /// </summary>
        /// <param name="profileName">Profile identifier.</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>Boolean based on the operation success.</returns>
        public async Task<bool> SetDisplayProfile(string profileName, CancellationToken ct)
        {
            Log.Trace($"Entering {nameof(SetDisplayProfile)}");
            const int lockTimeoutMs = 2000; // Timeout for the 
            var reserved = false;
            try
            {
                using (var cts = new CancellationTokenSource(lockTimeoutMs))
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ct))
                {
                    Log.Debug($"Attempting to enter semaphore, timeout {lockTimeoutMs}ms.");
                    await _profileSwitchLock.WaitAsync(linkedCts.Token); // Throws if not passed after cts timeout.
                    reserved = true;
                    Log.Trace("Semaphore entered.");
                    // TODO: Actually set profile.
                    return true;
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Exception occurred while setting new display profile.");
                return false;
            }
            finally
            {
                if (reserved) // Release lock if it was reserved.
                {
                    _profileSwitchLock.Release();
                }
            }
        }
        /// <summary>
        /// Get retrieved display information as a string.
        /// </summary>
        /// <returns></returns>
        public string GetRetrievedDisplayDataString()
        {
            return _displays.GetRetrievedDisplayInformationString();
        }

        /// <summary>
        /// Dispose the controller.
        /// </summary>
        public void Dispose()
        {
            Log.Trace("Disposing..");
            _backgroundLoopCts?.Cancel();
            _backgroundLoopCts?.Dispose();
            _profileSwitchLock?.Dispose();
            _hotkeys?.Dispose(); // Dispose the hotkeys handler.
            Log.Trace("Disposed.");
        }
    }
}
