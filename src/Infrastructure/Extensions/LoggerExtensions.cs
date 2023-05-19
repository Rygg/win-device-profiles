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
            EventName = nameof(HotKeyEventReceived),
            Level = LogLevel.Trace,
            Message = "HotKey event received: Modifiers: {Modifiers}, Key: {Key}, RegistrationId: {RegistrationId}"
        )
    ]
    public static partial void HotKeyEventReceived(this ILogger logger, SupportedKeyModifiers modifiers, SupportedKeys key, int registrationId);

    [LoggerMessage(
            EventId = 404,
            EventName = nameof(DisplayRetrieved),
            Level = LogLevel.Trace,
            Message = "Located DisplayDevice: Id: {DisplayId} - {DisplayDevice}"
        )
    ]
    public static partial void DisplayRetrieved(this ILogger logger, uint displayId, string displayDevice);

    [LoggerMessage(
            EventId = 405,
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

    // ERROR:

    [LoggerMessage(
            EventId = 451,
            EventName = nameof(ReceivedHotKeyEventNotRegistered),
            Level = LogLevel.Error,
            Message = "Registered hot key not found for the received event: {EventArgs}"
        )
    ]
    public static partial void ReceivedHotKeyEventNotRegistered(this ILogger logger, HotKeyEventArgs eventArgs);
}