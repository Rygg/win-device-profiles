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
            Name = profile.GetSection("Name").Value;
            HotKey = DeviceProfileHotKey.FromConfigurationSection(profile.GetSection("HotKey")); // Get hotkey section. Not required.
            // Get display settings:
            DisplaySettings = DeviceProfileDisplaySettings.ArrayFromDisplaySettingsConfigurationSection(profile.GetRequiredSection("DisplaySettings"));
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
        internal Keys Key { get; }

        /// <summary>
        /// Build from configuration section.
        /// </summary>
        /// <param name="hotkeySection">HotKey section</param>
        /// <exception cref="InvalidOperationException">Parsing failed.</exception>
        internal static DeviceProfileHotKey? FromConfigurationSection(IConfigurationSection hotkeySection)
        {
            if (!hotkeySection.Exists())
            {
                return null;
            }
            // Try to parse key.
            if (!Enum.TryParse(typeof(Keys), hotkeySection.GetSection("Key")?.Value, out var keyValue))
            {
                throw new InvalidOperationException("Key could not be parsed.");
            }
            var key = (Keys)(keyValue ?? throw new InvalidOperationException("Key was parsed to null.")); // Key found, set it.

            var modifiers = hotkeySection.GetSection("Modifiers"); // Get modifier flags.
            var ctrl = Convert.ToBoolean(modifiers.GetSection("Ctrl").Value);
            var alt = Convert.ToBoolean(modifiers.GetSection("Alt").Value);
            var shift = Convert.ToBoolean(modifiers.GetSection("Shift").Value);
            var win = Convert.ToBoolean(modifiers.GetSection("Win").Value);
            var mods = EKeyModifiers.None
                | (ctrl ? EKeyModifiers.Ctrl : EKeyModifiers.None)
                | (alt ? EKeyModifiers.Alt : EKeyModifiers.None)
                | (shift ? EKeyModifiers.Shift : EKeyModifiers.None)
                | (win ? EKeyModifiers.Win : EKeyModifiers.None);

            return new DeviceProfileHotKey(key, mods);
        }
        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="mods">Modifiers</param>
        private DeviceProfileHotKey(Keys key, EKeyModifiers mods)
        {
            Key = key;
            Modifiers = mods;
        }

        /// <summary>
        /// Returns a standard hotkey format.
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
        /// Builds an array of display settings from the configuration parameter.
        /// </summary>
        /// <param name="displaySetting"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Parsing failed.</exception>
        internal static DeviceProfileDisplaySettings[] ArrayFromDisplaySettingsConfigurationSection(IConfigurationSection displaySetting)
        {
            var children = displaySetting.GetChildren().ToArray();
            if (!children.Any())
            {
                throw new InvalidOperationException("DisplaySettings not be parsed.");
            }

            return (
                from child in children
                    let displayId = uint.Parse(child.GetRequiredSection("DisplayId").Value)
                    let primaryStr = child.GetSection("Primary")?.Value
                    let primaryDisplay = (bool?)(primaryStr != null ? Convert.ToBoolean(primaryStr) : null)
                    let hdrStr = child.GetSection("HDR")?.Value
                    let enableHdr = (bool?)(hdrStr != null ? Convert.ToBoolean(hdrStr) : null)
                    let refreshRateStr = child.GetSection("RefreshRate")?.Value
                    let refreshRate = (int?)(refreshRateStr != null ? Convert.ToInt32(refreshRateStr) : null)
                select new DeviceProfileDisplaySettings(displayId, primaryDisplay, enableHdr, refreshRate)
                ).ToArray();
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="displayId">DisplayId</param>
        /// <param name="primary">PrimaryDisplay</param>
        /// <param name="hdr">EnableHdr</param>
        /// <param name="refreshRate">RefreshRate</param>
        private DeviceProfileDisplaySettings(uint displayId, bool? primary, bool? hdr, int? refreshRate)
        {
            DisplayId = displayId;
            PrimaryDisplay = primary;
            EnableHdr = hdr;
            RefreshRate = refreshRate;
        }
    }
}
