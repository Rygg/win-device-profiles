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

        if (HotKey?.Validate() == false)
        {
            return false;
        }
        return true; // TODO: Display.
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
        };
    }

}

/// <summary>
/// Model for the HotKey options in the options file.
/// </summary>
public sealed record HotKeyOptions
{ 
    public SupportedKeys Key { get; init; }
    public ModifierOptions Modifiers { get; init; } = new();

    /// <summary>
    /// Validate.
    /// </summary>
    public bool Validate()
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
    public bool Ctrl { get; init; }
    public bool Shift { get; init; }
    public bool Alt { get; init; }
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