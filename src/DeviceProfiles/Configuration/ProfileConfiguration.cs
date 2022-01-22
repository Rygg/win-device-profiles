using System;
using System.Linq;
using DeviceProfiles.Classes;
using Microsoft.Extensions.Configuration;
using NLog;

namespace DeviceProfiles.Configuration
{
    /// <summary>
    /// Static class for reading DeviceConfiguration.
    /// </summary>
    internal static class ProfileConfiguration
    {
        /// <summary>
        /// Current class logger.
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Retrieves an array of device profiles from the given filename.
        /// </summary>
        /// <param name="fileName">Filename to be parsed.</param>
        /// <returns>An array of available DeviceProfiles</returns>
        internal static DeviceProfile[] GetConfiguredDeviceProfiles(string fileName)
        {
            try
            {
                Log.Debug($"Parsing DeviceProfiles from {fileName}");
                var deviceProfileConfiguration = new ConfigurationBuilder()
                    .AddJsonFile(fileName, true)
                    .Build();
                
                var deviceProfiles = deviceProfileConfiguration.GetRequiredSection("Profiles").GetChildren()?.ToArray();
                if (deviceProfiles?.Any() == true)
                {
                    var profiles = deviceProfiles.Select(c => new DeviceProfile(c)).ToArray();
                    Log.Debug($"Parsed {profiles.Length} device profiles from profile configuration file. Validating.");
                    ValidateConfiguration(profiles);
                    Log.Debug("Valid.");
                    return profiles;
                }
                Log.Debug("No DeviceProfiles found");
                return Array.Empty<DeviceProfile>();

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Exception occurred while parsing DeviceProfiles from {fileName}");
                return Array.Empty<DeviceProfile>();
            }
        }

        /// <summary>
        /// Validates the configuration. Throws an exception if configuration is invalid.
        /// </summary>
        /// <param name="profiles"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void ValidateConfiguration(DeviceProfile[] profiles)
        {
            // Check that there is a correct amount of unique profile ids.
            var uniqueIds = profiles.DistinctBy(p => p.Id).ToArray().Length == profiles.Length; 
            if (!uniqueIds)
            {
                throw new InvalidOperationException("ProfileIds were not unique.");
            }
            // Check for more than one primary monitor.
            if (profiles.Any(profile => profile.DisplaySettings.Count(ds => ds.PrimaryDisplay == true) > 1))
            {
                throw new InvalidOperationException("More than one primary monitor set in a profile.");
            }
            // Check if displayIds are not unique in within a single profile.
            if (profiles.Select(profile => profile.DisplaySettings.Select(ds => ds.DisplayId).ToArray()).Any(displayIds => displayIds.Length != displayIds.Distinct().Count()))
            {
                throw new InvalidOperationException("DisplayIds were not unique within a profile.");
            }
            // Get not-null hotkeys with the same key identifiers.
            var nonUniqueKeyModifiers = profiles.Where(profile => profile.HotKey != null).Select(profile => profile.HotKey!) // Get HotKeys.
                .GroupBy(key => key.Key) // Group by Key as it's a required value.
                .Where(k => k.Count() > 1) // Select groups with more than one value.
                .SelectMany(g => g.Select(k => k.Modifiers)).ToArray(); // Select each keys modifiers to an array.
            if (nonUniqueKeyModifiers.Length != nonUniqueKeyModifiers.Distinct().Count()) // Make sure that the modifiers differ.
            {
                throw new InvalidOperationException("HotKeys were not unique within the profile file.");
            }
        }
    }
}
