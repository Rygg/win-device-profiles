namespace Domain.Models;
public sealed record DeviceProfile
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
    public ICollection<DisplaySettings> DisplaySettings { get; init; } = Array.Empty<DisplaySettings>();
}