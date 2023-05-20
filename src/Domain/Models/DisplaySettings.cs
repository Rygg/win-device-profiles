namespace Domain.Models;

/// <summary>
/// Model for DisplaySettings.
/// </summary>
public sealed record DisplaySettings
{
    /// <summary>
    /// Display identifier to be used to link the display to the Environment structures.
    /// </summary>
    public uint DisplayId { get; init; }

    /// <summary>
    /// Should this display be set as primary. Null for no change.
    /// </summary>
    public bool? PrimaryDisplay { get; init; }

    /// <summary>
    /// Should HDR be enabled for this display. Null for no change.
    /// </summary>
    public bool? EnableHdr { get; init; }

    /// <summary>
    /// Should RefreshRate be switched for this display. Null for no change.
    /// </summary>
    public int? RefreshRate { get; init; }

}