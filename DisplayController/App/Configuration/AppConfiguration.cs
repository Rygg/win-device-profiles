using DisplayController.App.Control.Types;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DisplayController.App.Configuration
{
    /// <summary>
    /// Class contains the configuration for the application.
    /// </summary>
    internal class AppConfiguration
    {
        /// <summary>
        /// Profile configuration.
        /// </summary>
        internal Profile[] Profiles { get; }
        /// <summary>
        /// NLog configuration.
        /// </summary>
        internal NLogLoggingConfiguration NLog { get; }
        /// <summary>
        /// Builds configuration from read Configuration section.
        /// </summary>
        /// <param name="configuration"></param>
        internal AppConfiguration(IConfigurationSection configuration)
        {
            Profiles = configuration.GetRequiredSection("Profiles").GetChildren().Select(c => new Profile(c)).ToArray(); // Required.
            NLog = new NLogLoggingConfiguration(configuration.GetRequiredSection("NLog")); // Required.
        }
    }
    /// <summary>
    /// Profile container.
    /// </summary>
    internal class Profile
    {
        /// <summary>
        /// Name of the profile. Required.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Hotkey for triggering the profile. Not required.
        /// </summary>
        public ProfileHotKey HotKey { get; }
        /// <summary>
        /// DisplaySettings for the profile. Required.
        /// </summary>
        public ProfileDisplaySetting[] DisplaySettings { get; }

        /// <summary>
        /// Build profile from the section.
        /// </summary>
        /// <param name="profile">Profile-section</param>
        internal Profile(IConfigurationSection profile)
        {
            Name = profile.GetRequiredSection("Name").Value;
            var hotKeys = new ProfileHotKey(profile.GetSection("HotKey")); // Get hotkey section. Not required.
            HotKey = hotKeys.Key != null ? hotKeys : null; // Set null if no hot key was found.
                                                           // Get display settings. Required.
            DisplaySettings = profile.GetRequiredSection("DisplaySettings").GetChildren().Select(c => new ProfileDisplaySetting(c)).ToArray();
        }
    }
    /// <summary>
    /// HotKey configuration. Contains flags for Modifiers and the actual key.
    /// </summary>
    internal class ProfileHotKey
    {
        /// <summary>
        /// Modifier keys required for the hotkey to trigger.
        /// </summary>
        public KeyModifiers Modifiers { get; }
        /// <summary>
        /// HotKey.
        /// </summary>
        public Keys? Key { get; }
        /// <summary>
        /// Build from configuration section.
        /// </summary>
        /// <param name="hotkeySection">HotKey section</param>
        public ProfileHotKey(IConfigurationSection hotkeySection)
        {
            // Try to parse key.
            if (!Enum.TryParse(typeof(Keys), hotkeySection.GetSection("Key")?.Value, out var keyValue))
            {
                Key = null; // Set key to null and return.
                return;
            }
            Key = (Keys)keyValue; // Key found, set it.
                                  // Get modifier flags.
            var modifiers = hotkeySection.GetSection("Modifiers");
            var ctrl = Convert.ToBoolean(modifiers.GetSection("Ctrl").Value);
            var alt = Convert.ToBoolean(modifiers.GetSection("Alt").Value);
            var shift = Convert.ToBoolean(modifiers.GetSection("Shift").Value);
            var win = Convert.ToBoolean(modifiers.GetSection("Win").Value);
            Modifiers |= Modifiers = KeyModifiers.None
                | (ctrl ? KeyModifiers.Ctrl : KeyModifiers.None)
                | (alt ? KeyModifiers.Alt : KeyModifiers.None)
                | (shift ? KeyModifiers.Shift : KeyModifiers.None)
                | (win ? KeyModifiers.Win : KeyModifiers.None);
        }
        /// <summary>
        /// Returns a regular hotkey format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var modifierStr = string.Empty;
            if(Modifiers != KeyModifiers.None)
            {
                modifierStr = Modifiers.ToString().Replace(", ", "+");
            } 
            return string.IsNullOrEmpty(modifierStr) ? Key.ToString() : modifierStr+"+"+Key.ToString();
        }
    }
    /// <summary>
    /// Display settings.
    /// </summary>
    internal class ProfileDisplaySetting
    {
        /// <summary>
        /// Display identifier to be used to link the display to the WinApi structures.
        /// </summary>
        public uint DisplayId { get; }
        /// <summary>
        /// Should this display be set as primary. Null for no change.
        /// </summary>
        public bool? PrimaryDisplay { get; } = null;
        /// <summary>
        /// Should HDR be enabled for this display. Null for no change.
        /// </summary>
        public bool? EnableHdr { get; } = null;
        /// <summary>
        /// Should RefreshRate be switched for this display. Null for no change.
        /// </summary>
        public int? RefreshRate { get; } = null;

        /// <summary>
        /// Build class from configuration section.
        /// </summary>
        /// <param name="displaySetting">The configuration section.</param>
        internal ProfileDisplaySetting(IConfigurationSection displaySetting)
        {
            // Get display identifier or throw.
            DisplayId = uint.Parse(displaySetting.GetRequiredSection("DisplayId").Value);
            // Others are optional.
            var primaryStr = displaySetting.GetSection("Primary")?.Value;
            PrimaryDisplay = primaryStr != null ? Convert.ToBoolean(primaryStr) : null;
            var hdrStr = displaySetting.GetSection("HDR")?.Value;
            EnableHdr = hdrStr != null ? Convert.ToBoolean(hdrStr) : null;
            var refreshRateStr = displaySetting.GetSection("RefreshRate")?.Value;
            RefreshRate = refreshRateStr != null ? Convert.ToInt32(refreshRateStr) : null;
        }
    }

}

