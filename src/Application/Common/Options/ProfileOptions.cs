using Domain.Enums;
using Domain.Models;

namespace Application.Common.Options;

/// <summary>
/// Configuration Model for ProfileOptions section.
/// </summary>
public sealed record ProfileOptions
{
    /// <summary>
    /// List of all configured profiles.
    /// </summary>
    public IEnumerable<DeviceProfileOptions> Profiles { get; init; } = new List<DeviceProfileOptions>();

    /// <summary>
    /// Validate the configuration file.
    /// </summary>
    public static bool Validate(ProfileOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return options.Profiles.All(p => p.Validate());
    }
}

/// <summary>
/// Model for DeviceProfiles in the options file.
/// </summary>
public sealed record DeviceProfileOptions
{
    /// <summary>
    /// Identifier of the profile.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Name of the profile.
    /// </summary>
    public string Name { get; init; } = string.Empty;
    /// <summary>
    /// HotKey configuration for the profile.
    /// </summary>
    public HotKeyOptions? HotKey { get; init; }
    /// <summary>
    /// Display configuration for the profile.
    /// </summary>
    public ICollection<DisplayOptions> DisplaySettings { get; init; } = new List<DisplayOptions>();


    /// <summary>
    /// Validate the profile.
    /// </summary>
    /// <returns></returns>
    public bool Validate()
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
        return new DeviceProfile
        {
            Id = Id,
            Name = Name,
            HotKey = HotKey?.ToHotKeyCombination(),
            DisplaySettings = DisplaySettings.Select(ds => ds.ToDisplaySettings()).ToArray(),
        };
    }
}

/// <summary>
/// Model for the HotKey options in the options file.
/// </summary>
public sealed record HotKeyOptions
{
    /// <summary>
    /// Selected key
    /// </summary>
    public SupportedKeys Key { get; init; }
    /// <summary>
    /// Modifiers to press along with the selected key.
    /// </summary>
    public ModifierOptions Modifiers { get; init; } = new();

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
public sealed record ModifierOptions
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
public sealed record DisplayOptions
{
    /// <summary>
    /// Display identifier to be used to link the display to the Environment structures.
    /// </summary>
    public uint DisplayId { get; init; }

    /// <summary>
    /// Should this display be set as primary. Null for no change.
    /// </summary>
    public bool? Primary { get; init; }

    /// <summary>
    /// Should HDR be enabled for this display. Null for no change.
    /// </summary>
    public bool? EnableHdr { get; init; }

    /// <summary>
    /// Should RefreshRate be switched for this display. Null for no change.
    /// </summary>
    public int? RefreshRate { get; init; }

    /// <summary>
    /// Validate this.
    /// </summary>
    public bool IsValid()
    {
        return Primary != null || EnableHdr != null || RefreshRate != null;
    }
    /// <summary>
    /// Convert to domain model.
    /// </summary>
    /// <returns></returns>
    public DisplaySettings ToDisplaySettings()
    {
        return new DisplaySettings
        {
            DisplayId = DisplayId,
            PrimaryDisplay = Primary,
            EnableHdr = EnableHdr,
            RefreshRate = RefreshRate,
        };
    }
}