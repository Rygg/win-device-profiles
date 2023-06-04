using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;
public sealed class DeviceProfile : BaseEntity
{
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
    public ICollection<DisplaySettings> DisplaySettings { get; init; } = new List<DisplaySettings>();

}