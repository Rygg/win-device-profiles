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
            var uniqueIds = profiles.DistinctBy(p => p.Id).ToArray().Length == profiles.Length; // Check that there is a correct amount of Ids.
            if (!uniqueIds)
            {
                throw new InvalidOperationException("ProfileIds were not unique.");
            }
        }
    }
}
