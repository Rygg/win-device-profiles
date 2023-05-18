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
            EventId = 611,
            EventName = nameof(StartingShutdown),
            Level = LogLevel.Debug,
            Message = "Starting to shut down the application..."
        )
    ]
    public static partial void StartingShutdown(this ILogger logger);

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
            EventName = nameof(AddedProfileToContextMenu),
            Level = LogLevel.Information,
            Message = "Added {ProfileText} to context menu profiles."
        )
    ]
    public static partial void AddedProfileToContextMenu(this ILogger logger, string profileText);

    [LoggerMessage(
            EventId = 623,
            EventName = nameof(CreatedContextMenu),
            Level = LogLevel.Debug,
            Message = "Created ContextMenu for the tray icon."
        )
    ]
    public static partial void CreatedContextMenu(this ILogger logger);

    [LoggerMessage(
            EventId = 651,
            EventName = nameof(EventHandlerTriggeredByWrongType),
            Level = LogLevel.Error,
            Message = "{EventHandlerName} triggered by wrong type. Ignoring request."
        )
    ]
    public static partial void EventHandlerTriggeredByWrongType(this ILogger logger, string eventHandlerName);
}