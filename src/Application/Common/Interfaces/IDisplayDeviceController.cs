using Domain.Entities;

namespace Application.Common.Interfaces;

/// <summary>
/// Interface for DisplayControllers
/// </summary>
public interface IDisplayDeviceController
{
    /// <summary>
    /// Changes the current system display settings to the given DeviceProfile.
    /// </summary>
    /// <param name="profile">DeviceProfile to be set as active.</param>
    /// <param name="cancellationToken">CancellationToken to cancel the operation.</param>
    /// <returns>True if the operation was successful. False if no changes were made.</returns>
    Task<bool> ChangeDisplaySettings(DeviceProfile profile, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets retrieved display information as a string.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken to cancel the operation.</param>
    /// <returns>A formatted string containing the current display information.</returns>
    Task<string> GetCurrentDisplayInformationString(CancellationToken cancellationToken);
}