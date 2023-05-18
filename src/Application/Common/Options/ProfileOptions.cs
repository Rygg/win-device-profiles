using Domain.Models;

namespace Application.Common.Options;

public sealed record ProfileOptions
{
    public const string RootSectionName = "Profiles";

    public IEnumerable<DeviceProfileOptions> Profiles { get; init; } = new List<DeviceProfileOptions>();

    public static bool Validate(ProfileOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        foreach (var profile in options.Profiles)
        {
            return DeviceProfileOptions.Validate(profile);
        }

        return true;
    }
}

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
    /// HotKey for triggering the profile. Not required.
    /// </summary>
    public HotKeyCombination? HotKey { get; init; }

    /// <summary>
    /// DisplaySettings for the profile. Required.
    /// </summary>
    //internal DeviceProfileDisplaySettings[] DisplaySettings { get; }

    public static bool Validate(DeviceProfileOptions options)
    {
        return true; // TODO:
    }
}