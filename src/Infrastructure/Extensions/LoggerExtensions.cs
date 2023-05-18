using Domain.Enums;
using Domain.Models;
using Infrastructure.Interfaces.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions;

public static partial class LoggerExtensions
{
    // TRACE:
    [LoggerMessage(
            EventId = 001,
            EventName = nameof(StartingDispose),
            Level = LogLevel.Trace,
            Message = "Starting dispose operation..."
        )
    ]
    public static partial void StartingDispose(this ILogger logger);

    [LoggerMessage(
            EventId = 002,
            EventName = nameof(DisposingCompleted),
            Level = LogLevel.Trace,
            Message = "Disposed."
        )
    ]
    public static partial void DisposingCompleted(this ILogger logger);

    [LoggerMessage(
            EventId = 003,
            EventName = nameof(HotKeyEventReceived),
            Level = LogLevel.Trace,
            Message = "HotKey event received: Modifiers: {Modifiers}, Key: {Key}, RegistrationId: {RegistrationId}"
        )
    ]
    public static partial void HotKeyEventReceived(this ILogger logger, SupportedKeyModifiers modifiers, SupportedKeys key, int registrationId);

    // DEBUG:

    [LoggerMessage(
            EventId = 101,
            EventName = nameof(RegisteringGlobalHotKey),
            Level = LogLevel.Debug,
            Message = "Registering global hot key for the application. HotKey Identifier: {KeyRegistrationId}, Key Combination: {KeyCombination}"
        )
    ]
    public static partial void RegisteringGlobalHotKey(this ILogger logger, int keyRegistrationId, HotKeyCombination keyCombination);

    [LoggerMessage(
            EventId = 102,
            EventName = nameof(UnregisterGlobalHotKey),
            Level = LogLevel.Debug,
            Message = "Unregister hot key registration: RegistrationId: {KeyRegistrationId}, Key Combination: {KeyCombination}"
        )
    ]
    public static partial void UnregisterGlobalHotKey(this ILogger logger, int keyRegistrationId, HotKeyCombination keyCombination);

    // INFO:

    [LoggerMessage(
            EventId = 201,
            EventName = nameof(KeyCombinationRegistered),
            Level = LogLevel.Information,
            Message = "Key combination successfully {KeyCombination} registered."
        )
    ]
    public static partial void KeyCombinationRegistered(this ILogger logger, HotKeyCombination keyCombination);

    // WARNING:

    [LoggerMessage(
            EventId = 301,
            EventName = nameof(KeyCombinationAlreadyRegistered),
            Level = LogLevel.Warning,
            Message = "Key combination {KeyCombination} is already registered. Returning."
        )
    ]
    public static partial void KeyCombinationAlreadyRegistered(this ILogger logger, HotKeyCombination keyCombination);

    // ERROR:

    [LoggerMessage(
            EventId = 401,
            EventName = nameof(ReceivedHotKeyEventNotRegistered),
            Level = LogLevel.Error,
            Message = "Registered hot key not found for the received event: {EventArgs}"
        )
    ]
    public static partial void ReceivedHotKeyEventNotRegistered(this ILogger logger, HotKeyEventArgs eventArgs);
}