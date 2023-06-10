using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace TrayApplication.Extensions;

public static partial class LoggerExtensions
{
    // TRACE:
    [LoggerMessage(
            EventId = 601,
            EventName = nameof(StartingDispose),
            Level = LogLevel.Trace,
            Message = "Starting dispose operation..."
        )
    ]
    public static partial void StartingDispose(this ILogger logger);

    [LoggerMessage(
            EventId = 602,
            EventName = nameof(DisposingCompleted),
            Level = LogLevel.Trace,
            Message = "Disposed."
        )
    ]
    public static partial void DisposingCompleted(this ILogger logger);

    [LoggerMessage(
            EventId = 603,
            EventName = nameof(HotKeyEventReceived),
            Level = LogLevel.Trace,
            Message = "HotKey event received: Modifiers: {Modifiers}, Key: {Key}, RegistrationId: {RegistrationId}"
        )
    ]
    public static partial void HotKeyEventReceived(this ILogger logger, SupportedKeyModifiers modifiers, SupportedKeys key, int registrationId);

    [LoggerMessage(
            EventId = 611,
            EventName = nameof(StartingShutdown),
            Level = LogLevel.Debug,
            Message = "Starting to shut down the application..."
        )
    ]
    public static partial void StartingShutdown(this ILogger logger);

    [LoggerMessage(
            EventId = 612,
            EventName = nameof(AddedProfileToContextMenu),
            Level = LogLevel.Debug,
            Message = "Added {ProfileText} to context menu profiles."
        )
    ]
    public static partial void AddedProfileToContextMenu(this ILogger logger, string profileText);

    [LoggerMessage(
            EventId = 613,
            EventName = nameof(CreatedContextMenu),
            Level = LogLevel.Debug,
            Message = "Created ContextMenu for the tray icon."
        )
    ]
    public static partial void CreatedContextMenu(this ILogger logger);
    [LoggerMessage(
            EventId = 614,
            EventName = nameof(GlobalTokenCancelled),
            Level = LogLevel.Debug,
            Message = "Application's CancellationTokenSource was cancelled."
        )
    ]
    public static partial void GlobalTokenCancelled(this ILogger logger);

    [LoggerMessage(
            EventId = 621,
            EventName = nameof(ApplicationShuttingDown),
            Level = LogLevel.Information,
            Message = "Application shutting down."
        )
    ]
    public static partial void ApplicationShuttingDown(this ILogger logger);

    [LoggerMessage(
            EventId = 622,
            EventName = nameof(CopiedInformationToClipboard),
            Level = LogLevel.Information,
            Message = "Copied device information to clipboard. {DeviceInformation}"
        )
    ]
    public static partial void CopiedInformationToClipboard(this ILogger logger, string deviceInformation);

    [LoggerMessage(
            EventId = 651,
            EventName = nameof(EventHandlerTriggeredByWrongType),
            Level = LogLevel.Error,
            Message = "{EventHandlerName} triggered by wrong type. Ignoring request."
        )
    ]
    public static partial void EventHandlerTriggeredByWrongType(this ILogger logger, string eventHandlerName);

    [LoggerMessage(
            EventId = 652,
            EventName = nameof(ProfilesCouldNotBeDeserialized),
            Level = LogLevel.Error,
            Message = "Profiles could not be deserialized from the given file input."
        )
    ]
    public static partial void ProfilesCouldNotBeDeserialized(this ILogger logger);
    [LoggerMessage(
            EventId = 653,
            EventName = nameof(ExceptionOnFileImport),
            Level = LogLevel.Error,
            Message = "Exception occurred while trying to import profiles from a file."
        )
    ]
    public static partial void ExceptionOnFileImport(this ILogger logger, Exception ex);
}