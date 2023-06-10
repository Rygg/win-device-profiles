using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace Application.Features.Profiles.Commands.Common;

/// <summary>
/// Data model for a profiles file.
/// </summary>
public sealed record ProfilesFileDto
{
    /// <summary>
    /// List of all configured profiles.
    /// </summary>
    public IReadOnlyCollection<DeviceProfileDto> Profiles { get; init; } = new List<DeviceProfileDto>();

    /// <summary>
    /// Validate the parsed file.
    /// </summary>
    public bool Validate()
    {
        if (!Profiles.Any())
        {
            return false;
        }
            
        if (!Profiles.All(p => p.IsValid()))
        {
            return false;
        }

        // Check Duplicate Profile Identifiers:
        if (Profiles
            .GroupBy(p => p.Id)
            .Any(g => g.Count() > 1)
            )
        {
            return false;
        }
        // Check duplicate hot key bindings.
        if (Profiles
            .Where(p => p.HotKey != null)
            .GroupBy(p => p.HotKey)
            .Any(g => g.Count() > 1)
            )
        {
            return false;
        }

        foreach (var profile in Profiles)
        {
            // Check duplicate display ids.
            if (profile.DisplaySettings
                .GroupBy(ds => ds.DisplayId)
                .Any(g => g.Count() > 1))
            {
                return false;
            }
            // Check duplicated primary displays.
            if (profile.DisplaySettings.Count(p => p.Primary == true) > 1)
            {
                return false;
            }
        }
        return true;
    }
}

/// <summary>
/// Model for DeviceProfiles in the options file.
/// </summary>
public sealed record DeviceProfileDto
{
    /// <summary>
    /// Identifier of the profile.
    /// </summary>
    public int? Id { get; init; }

    /// <summary>
    /// Name of the profile.
    /// </summary>
    public string? Name { get; init; }
    /// <summary>
    /// HotKey configuration for the profile.
    /// </summary>
    public HotKeyDto? HotKey { get; init; }
    /// <summary>
    /// Display configuration for the profile.
    /// </summary>
    public IReadOnlyCollection<DisplaySettingsDto> DisplaySettings { get; init; } = new List<DisplaySettingsDto>();


    /// <summary>
    /// Validate the profile.
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        if (Id == default)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            return false;
        }

        // Validate HotKey and all DisplaySettings.
        return HotKey?.IsValid() != false && DisplaySettings.All(ds => ds.IsValid());
    }

    /// <summary>
    /// Transform this configuration model to the Domain level <see cref="DeviceProfile"/>.
    /// </summary>
    public DeviceProfile ToDeviceProfile()
    {
        if (!IsValid())
        {
            throw new InvalidOperationException("Configuration is not valid.");
        }

        return new DeviceProfile
        {
            Id = Id!.Value,
            Name = Name!,
            HotKey = HotKey?.ToHotKeyCombination(),
            DisplaySettings = DisplaySettings.Select(ds => ds.ToDisplaySettings()).ToList(),
        };
    }
}

/// <summary>
/// Model for the HotKey options in the options file.
/// </summary>
public sealed record HotKeyDto
{
    /// <summary>
    /// Selected key
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SupportedKeys Key { get; init; }
    /// <summary>
    /// Modifiers to press along with the selected key.
    /// </summary>
    public ModifierDto Modifiers { get; init; } = new();

    /// <summary>
    /// Validate.
    /// </summary>
    public bool IsValid()
    {
        return Key != SupportedKeys.None;
    }

    /// <summary>
    /// Convert this setting model to a domain level <see cref="HotKeyCombination"/>
    /// </summary>
    /// <returns></returns>
    public HotKeyCombination ToHotKeyCombination()
    {
        if (!IsValid())
        {
            throw new InvalidOperationException("Configuration is not valid.");
        }

        return new HotKeyCombination
        {
            Key = Key,
            Modifiers = Modifiers.ToSupportedKeyModifiers()
        };
    }
}

/// <summary>
/// Modifier options model.
/// </summary>
public sealed record ModifierDto
{
    /// <summary>
    /// Ctrl is required.
    /// </summary>
    public bool Ctrl { get; init; }
    /// <summary>
    /// Shift is required.
    /// </summary>
    public bool Shift { get; init; }
    /// <summary>
    /// Alt is required.
    /// </summary>
    public bool Alt { get; init; }
    /// <summary>
    /// Windows Key is required.
    /// </summary>
    public bool Win { get; init; }

    /// <summary>
    /// Convert this options model into the domain level flagged enumeration value.
    /// </summary>
    /// <returns></returns>
    public SupportedKeyModifiers ToSupportedKeyModifiers()
    {
        return SupportedKeyModifiers.None
               | (Ctrl ? SupportedKeyModifiers.Ctrl : SupportedKeyModifiers.None)
               | (Alt ? SupportedKeyModifiers.Alt : SupportedKeyModifiers.None)
               | (Shift ? SupportedKeyModifiers.Shift : SupportedKeyModifiers.None)
               | (Win ? SupportedKeyModifiers.Win : SupportedKeyModifiers.None);
    }
}


/// <summary>
/// Display settings.
/// </summary>
public sealed record DisplaySettingsDto
{
    /// <summary>
    /// Display identifier to be used to link the display to the Environment structures.
    /// </summary>
    public uint? DisplayId { get; init; }

    /// <summary>
    /// Should this display be set as primary. Null for no change.
    /// </summary>
    public bool? Primary { get; init; }

    /// <summary>
    /// Should HDR be enabled for this display. Null for no change.
    /// </summary>
    public bool? Hdr { get; init; }

    /// <summary>
    /// Should RefreshRate be switched for this display. Null for no change.
    /// </summary>
    public int? RefreshRate { get; init; }

    /// <summary>
    /// Validate this.
    /// </summary>
    public bool IsValid()
    {
        if (DisplayId == null)
        {
            return false;
        }
        return Primary != null || Hdr != null || RefreshRate != null;
    }
    /// <summary>
    /// Convert to domain model.
    /// </summary>
    /// <returns></returns>
    public DisplaySettings ToDisplaySettings()
    {
        if (!IsValid())
        {
            throw new InvalidOperationException("Configuration is not valid.");
        }

        return new DisplaySettings
        {
            DisplayId = DisplayId!.Value, // This is not null because configuration is validated.
            PrimaryDisplay = Primary,
            EnableHdr = Hdr,
            RefreshRate = RefreshRate,
        };
    }
}
