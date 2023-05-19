using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeviceProfiles.Classes;
using DeviceProfiles.DeviceControllers;
using DeviceProfiles.Enums;
using NLog;

namespace DeviceProfiles.Application
{
    /// <summary>
    /// MainController class responsible for the application functionality.
    /// </summary>
    internal sealed class ApplicationControl : IDisposable
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
        private DeviceProfile[] _displayProfiles;
        /// <summary>
        /// CancellationTokenSource controlling the background loop.
        /// </summary>
        private CancellationTokenSource? _backgroundLoopCts;
        /// <summary>
        /// Lock to prevent concurrent profile switching.
        /// </summary>
        private readonly SemaphoreSlim _profileSwitchLock = new(1, 1);
        /// <summary>
        /// Default constructor for the MainController.
        /// </summary>
        internal ApplicationControl()
        {
            _displays = new DisplayController();
            _hotkeys = new HotKeyController();
            _displayProfiles = Array.Empty<DeviceProfile>();
        }
        /// <summary>
        /// Destructor for Disposing.
        /// </summary>
        ~ApplicationControl()
        {
            Dispose();
        }

        /// <summary>
        /// Method starts the controller functionality with the configuration given as a parameter.
        /// </summary>
        /// <param name="profiles">Configuration for the controller.</param>
        internal void Start(DeviceProfile[] profiles)
        {
            Log.Debug("Starting MainController functionality.");
            _displayProfiles = profiles; // Save profiles.
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
            
            foreach(var profile in _displayProfiles.Where(p => p.HotKey?.Key != null)) // Loop through profiles with hotkeys.
            {
                Log.Debug($"Registering global hotkey for profile: {profile.Id}-{profile.Name}");
                _hotkeys.RegisterHotKey(profile.Id, profile.HotKey!); // Register configured hotkey.
            }
            Log.Debug("HotKeys registered");
            while(!_backgroundLoopCts.Token.IsCancellationRequested)
            {
                var registrationEvent = await _hotkeys.GetHotKeyPressAsync(_backgroundLoopCts.Token);
                _backgroundLoopCts.Token.ThrowIfCancellationRequested(); // Throw from the loop if cancelled.
                Log.Debug($"HotKeyPress detected: ProfileId: {registrationEvent.ProfileId}, Modifiers: {(EKeyModifiers)registrationEvent.Modifiers}, Key: {(Keys)registrationEvent.Key}");
                Log.Info($"User switched profile by hotkey. Selected profileId: {registrationEvent.ProfileId}");

                if(await SetDisplayProfile(registrationEvent.ProfileId, _backgroundLoopCts.Token)) // Set the requested profile.
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
        /// <param name="profileId">Profile identifier.</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>Boolean based on the operation success.</returns>
        public async Task<bool> SetDisplayProfile(int profileId, CancellationToken ct)
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
                    Log.Trace("Semaphore entered.");
                    reserved = true;
                    // Get profile data.
                    var profileData = _displayProfiles.FirstOrDefault(p => p.Id == profileId);
                    if(profileData == null)
                    {
                        Log.Error($"No profile data found for profile {profileId}");
                        return false;
                    }
                    // Otherwise, apply the display profile.
                    Log.Info($"Applying Profile {profileId}-{profileData.Name}");
                    _displays.SetDisplayProfile(profileData.DisplaySettings);
                    Log.Info($"Profile applied.");
                    return true; // Return true if no exceptions.
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
                    Log.Trace("Semaphore released.");
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
            _profileSwitchLock.Dispose();
            _hotkeys.Dispose(); // Dispose the hotkeys handler.
            Log.Trace("Disposed.");
            GC.SuppressFinalize(this);
        }
    }
}
