using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DeviceProfiles.Enums;
using DeviceProfiles.Resources.Text;
using Microsoft.Extensions.Configuration;

namespace DeviceProfiles.Classes
{
    /// <summary>
    /// Class containing DeviceProfile
    /// </summary>
    internal class DeviceProfile
    {
        /// <summary>
        /// Identifier of the profile.
        /// </summary>
        internal int Id { get; }
        /// <summary>
        /// Name of the profile.
        /// </summary>
        internal string Name { get; }
        /// <summary>
        /// HotKey for triggering the profile. Not required.
        /// </summary>
        internal DeviceProfileHotKey? HotKey { get; }
        /// <summary>
        /// DisplaySettings for the profile. Required.
        /// </summary>
        internal DeviceProfileDisplaySettings[] DisplaySettings { get; }
        /// <summary>
        /// Construct the class from configuration.
        /// </summary>
        /// <param name="profile">Profile-section</param>
        internal DeviceProfile(IConfiguration profile)
        {
            Id = int.Parse(profile.GetRequiredSection("Id").Value); // Required.
            Name = profile.GetRequiredSection("Name").Value;
            var hotKeys = new DeviceProfileHotKey(profile.GetSection("HotKey")); // Get hotkey section. Not required.
            HotKey = hotKeys.Key != null ? hotKeys : null; // Set null if no hot key was found.
                                                           // Get display settings. Required.
            DisplaySettings = profile.GetRequiredSection("DisplaySettings").GetChildren().Select(c => new DeviceProfileDisplaySettings(c)).ToArray();
        }
        /// <summary>
        /// Method returns a summary string for the profile.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var str = Name;
            var displays = new List<string>();
            foreach (var displaySetting in DisplaySettings)
            {
                var str2 = $"Display {displaySetting.DisplayId}: ";
                var details = new List<string>();
                if (displaySetting.PrimaryDisplay == true)
                {
                    details.Add(Strings.ProfileSummaryPrimary);
                }
                if (displaySetting.EnableHdr.HasValue)
                {
                    details.Add(displaySetting.EnableHdr == true ? Strings.ProfileSummaryHdr : Strings.ProfileSummarySdr);
                }
                if (displaySetting.RefreshRate.HasValue)
                {
                    details.Add($"{displaySetting.RefreshRate.Value}{Strings.ProfileSummaryHz}");
                }
                str2 += string.Join('/',details.ToArray());
                displays.Add(str2);
            }

            str += $" ({string.Join(", ", displays.ToArray())})";
            return str;
        }
    }
    /// <summary>
    /// HotKey configuration. Contains flags for Modifiers and the actual key.
    /// </summary>
    internal class DeviceProfileHotKey
    {
        /// <summary>
        /// Modifier keys required for the hotkey to trigger.
        /// </summary>
        internal EKeyModifiers Modifiers { get; }
        /// <summary>
        /// HotKey.
        /// </summary>
        internal Keys? Key { get; }
        /// <summary>
        /// Build from configuration section.
        /// </summary>
        /// <param name="hotkeySection">HotKey section</param>
        internal DeviceProfileHotKey(IConfiguration hotkeySection)
        {
            // Try to parse key.
            if (!Enum.TryParse(typeof(Keys), hotkeySection.GetSection("Key")?.Value, out var keyValue))
            {
                Key = null; // Set key to null and return.
                return;
            }
            Key = (Keys)(keyValue ?? throw new InvalidOperationException("Key was parsed to null.")); // Key found, set it.

            var modifiers = hotkeySection.GetSection("Modifiers"); // Get modifier flags.
            var ctrl = Convert.ToBoolean(modifiers.GetSection("Ctrl").Value);
            var alt = Convert.ToBoolean(modifiers.GetSection("Alt").Value);
            var shift = Convert.ToBoolean(modifiers.GetSection("Shift").Value);
            var win = Convert.ToBoolean(modifiers.GetSection("Win").Value);
            Modifiers |= Modifiers = EKeyModifiers.None
                | (ctrl ? EKeyModifiers.Ctrl : EKeyModifiers.None)
                | (alt ? EKeyModifiers.Alt : EKeyModifiers.None)
                | (shift ? EKeyModifiers.Shift : EKeyModifiers.None)
                | (win ? EKeyModifiers.Win : EKeyModifiers.None);
        }
        /// <summary>
        /// Returns a regular hotkey format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var modifierStr = string.Empty;
            if (Modifiers != EKeyModifiers.None)
            {
                modifierStr = Modifiers.ToString().Replace(", ", "+");
            }
            return (string.IsNullOrEmpty(modifierStr) ? Key.ToString() : modifierStr + "+" + Key) ?? string.Empty;
        }
    }
    /// <summary>
    /// Display settings.
    /// </summary>
    internal class DeviceProfileDisplaySettings
    {
        /// <summary>
        /// Display identifier to be used to link the display to the WinApi structures.
        /// </summary>
        internal uint DisplayId { get; }
        /// <summary>
        /// Should this display be set as primary. Null for no change.
        /// </summary>
        internal bool? PrimaryDisplay { get; } = null;
        /// <summary>
        /// Should HDR be enabled for this display. Null for no change.
        /// </summary>
        internal bool? EnableHdr { get; } = null;
        /// <summary>
        /// Should RefreshRate be switched for this display. Null for no change.
        /// </summary>
        internal int? RefreshRate { get; } = null;

        /// <summary>
        /// Build class from configuration section.
        /// </summary>
        /// <param name="displaySetting">The configuration section.</param>
        internal DeviceProfileDisplaySettings(IConfiguration displaySetting)
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
