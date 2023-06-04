using Domain.Enums;
using Domain.Models;
using Infrastructure.Interfaces.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions;

public static partial class LoggerExtensions
{
    // TRACE:
    [LoggerMessage(
            EventId = 401,
            EventName = nameof(StartingDispose),
            Level = LogLevel.Trace,
            Message = "Starting dispose operation..."
        )
    ]
    public static partial void StartingDispose(this ILogger logger);

    [LoggerMessage(
            EventId = 402,
            EventName = nameof(DisposingCompleted),
            Level = LogLevel.Trace,
            Message = "Disposed."
        )
    ]
    public static partial void DisposingCompleted(this ILogger logger);
    
    [LoggerMessage(
            EventId = 403,
            EventName = nameof(DisplayRetrieved),
            Level = LogLevel.Trace,
            Message = "Located DisplayDevice: Id: {DisplayId} - {DisplayDevice}"
        )
    ]
    public static partial void DisplayRetrieved(this ILogger logger, uint displayId, string displayDevice);

    [LoggerMessage(
            EventId = 404,
            EventName = nameof(DeviceModeRetrieved),
            Level = LogLevel.Trace,
            Message = "Retrieved DeviceModes: Id: {DisplayId} - {DeviceMode}"
        )
    ]
    public static partial void DeviceModeRetrieved(this ILogger logger, uint displayId, string deviceMode);

    // DEBUG:

    [LoggerMessage(
            EventId = 411,
            EventName = nameof(RegisteringGlobalHotKey),
            Level = LogLevel.Debug,
            Message = "Registering global hot key for the application. HotKey Identifier: {KeyRegistrationId}, Key Combination: {KeyCombination}"
        )
    ]
    public static partial void RegisteringGlobalHotKey(this ILogger logger, int keyRegistrationId, HotKeyCombination keyCombination);

    [LoggerMessage(
            EventId = 412,
            EventName = nameof(UnregisterGlobalHotKey),
            Level = LogLevel.Debug,
            Message = "Unregister hot key registration: RegistrationId: {KeyRegistrationId}, Key Combination: {KeyCombination}"
        )
    ]
    public static partial void UnregisterGlobalHotKey(this ILogger logger, int keyRegistrationId, HotKeyCombination keyCombination);

    [LoggerMessage(
            EventId = 413,
            EventName = nameof(RetrievingDisplayDevices),
            Level = LogLevel.Debug,
            Message = "Retrieving DisplayDevices.."
        )
    ]
    public static partial void RetrievingDisplayDevices(this ILogger logger);

    [LoggerMessage(
            EventId = 414,
            EventName = nameof(DeviceNotAttachedToDesktop),
            Level = LogLevel.Debug,
            Message = "Device is not attached to the desktop. Ignoring"
        )
    ]
    public static partial void DeviceNotAttachedToDesktop(this ILogger logger);

    [LoggerMessage(
            EventId = 415,
            EventName = nameof(RetrievingDeviceModes),
            Level = LogLevel.Debug,
            Message = "Retrieving DeviceModes for display {DisplayId}"
        )
    ]
    public static partial void RetrievingDeviceModes(this ILogger logger, uint displayId);

    [LoggerMessage(
            EventId = 416,
            EventName = nameof(RetrievingAdvancedDisplayInformation),
            Level = LogLevel.Debug,
            Message = "Retrieving advanced display information."
        )
    ]
    public static partial void RetrievingAdvancedDisplayInformation(this ILogger logger);

    [LoggerMessage(
            EventId = 417,
            EventName = nameof(RetrievedDisplayInformation),
            Level = LogLevel.Debug,
            Message = "DisplayId: {DisplayId}, Data: {RetrievedDisplayData}"
        )
    ]
    public static partial void RetrievedDisplayInformation(this ILogger logger, uint displayId, string retrievedDisplayData);

    [LoggerMessage(
            EventId = 418,
            EventName = nameof(UpdatedRegistrySettings),
            Level = LogLevel.Debug,
            Message = "Updated registry settings for monitor {DisplayId}"
        )
    ]
    public static partial void UpdatedRegistrySettings(this ILogger logger, uint displayId);

    // INFO:

    [LoggerMessage(
            EventId = 421,
            EventName = nameof(KeyCombinationRegistered),
            Level = LogLevel.Information,
            Message = "Key combination successfully {KeyCombination} registered."
        )
    ]
    public static partial void KeyCombinationRegistered(this ILogger logger, HotKeyCombination keyCombination);

    [LoggerMessage(
            EventId = 422,
            EventName = nameof(DisplaysRefreshed),
            Level = LogLevel.Information,
            Message = "Displays refreshed."
        )
    ]
    public static partial void DisplaysRefreshed(this ILogger logger);

    [LoggerMessage(
            EventId = 423,
            EventName = nameof(SettingDisplayAsPrimary),
            Level = LogLevel.Information,
            Message = "Setting Display {DisplayId} as the new primary display."
        )
    ]
    public static partial void SettingDisplayAsPrimary(this ILogger logger, uint displayId);

    [LoggerMessage(
            EventId = 424,
            EventName = nameof(DisplayWasAlreadyPrimary),
            Level = LogLevel.Information,
            Message = "Display {DisplayId} was already the primary display. Skipping operation."
        )
    ]
    public static partial void DisplayWasAlreadyPrimary(this ILogger logger, uint displayId);

    [LoggerMessage(
            EventId = 425,
            EventName = nameof(SettingDisplayRefreshRate),
            Level = LogLevel.Information,
            Message = "Setting Display {DisplayId} RefreshRate to {NewRefreshRate}Hz."
        )
    ]
    public static partial void SettingDisplayRefreshRate(this ILogger logger, uint displayId, int? newRefreshRate);

    [LoggerMessage(
            EventId = 426,
            EventName = nameof(UpdatingAdvancedColorState),
            Level = LogLevel.Information,
            Message = "Updating the Advanced Color State for Display {DisplayId}. New State: {newState}"
        )
    ]
    public static partial void UpdatingAdvancedColorState(this ILogger logger, uint displayId, bool newState);

    [LoggerMessage(
            EventId = 427,
            EventName = nameof(AdvancedColorStateAlreadySet),
            Level = LogLevel.Information,
            Message = "Advanced color mode was already set to the desired state for monitor {DisplayId}. Returning."
        )
    ]
    public static partial void AdvancedColorStateAlreadySet(this ILogger logger, uint displayId);

    [LoggerMessage(
            EventId = 428,
            EventName = nameof(AdvancedColorStateUpdated),
            Level = LogLevel.Information,
            Message = "Advanced color state was updated for Display {DisplayId}."
        )
    ]
    public static partial void AdvancedColorStateUpdated(this ILogger logger, uint displayId);

    // WARNING:

    [LoggerMessage(
            EventId = 441,
            EventName = nameof(KeyCombinationAlreadyRegistered),
            Level = LogLevel.Warning,
            Message = "Key combination {KeyCombination} is already registered. Returning."
        )
    ]
    public static partial void KeyCombinationAlreadyRegistered(this ILogger logger, HotKeyCombination keyCombination);
    
    [LoggerMessage(
            EventId = 442,
            EventName = nameof(RetrievedDeviceSourceNotFound),
            Level = LogLevel.Warning,
            Message = "Retrieved device source was not located in originally retrieved display information. Skipping the device."
        )
    ]
    public static partial void RetrievedDeviceSourceNotFound(this ILogger logger);
        
    [LoggerMessage(
            EventId = 443,
            EventName = nameof(PrimaryDisplayAlreadySet),
            Level = LogLevel.Warning,
            Message = "Primary Display was already set for this profile. Ignoring primary change for display {DisplayId}."
        )
    ]
    public static partial void PrimaryDisplayAlreadySet(this ILogger logger, uint displayId);

    [LoggerMessage(
            EventId = 444,
            EventName = nameof(DisplayNotFound),
            Level = LogLevel.Warning,
            Message = "Display with Id {DisplayId} not found. Returning."
        )
    ]
    public static partial void DisplayNotFound(this ILogger logger, uint displayId);

    // ERROR:

    [LoggerMessage(
            EventId = 451,
            EventName = nameof(ReceivedHotKeyEventNotRegistered),
            Level = LogLevel.Error,
            Message = "Registered hot key not found for the received event: {EventArgs}"
        )
    ]
    public static partial void ReceivedHotKeyEventNotRegistered(this ILogger logger, HotKeyEventArgs eventArgs);

    [LoggerMessage(
            EventId = 452,
            EventName = nameof(RefreshRateNotSupported),
            Level = LogLevel.Error,
            Message = "The desired refresh rate is not supported for this monitor. DisplayId: {DisplayId}, RefreshRate: {RefreshRate}"
        )
    ]
    public static partial void RefreshRateNotSupported(this ILogger logger, uint displayId, int? refreshRate);

    [LoggerMessage(
            EventId = 453,
            EventName = nameof(CannotSetAdvancedColorMode),
            Level = LogLevel.Error,
            Message = "Cannot set Advanced Color Mode for Display {DisplayId}. Required information is missing or the monitor doesn't support Advanced Color Mode."
        )
    ]
    public static partial void CannotSetAdvancedColorMode(this ILogger logger, uint displayId);
}