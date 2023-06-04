using Microsoft.Extensions.Logging;

namespace Application.Common.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(
            EventId = 221,
            EventName = nameof(DisplayProfileChanged),
            Level = LogLevel.Information,
            Message = "DisplayProfile successfully activated."
        )
    ]
    public static partial void DisplayProfileChanged(this ILogger logger);

    [LoggerMessage(
            EventId = 250,
            EventName = nameof(UnhandledExceptionOccurred),
            Level = LogLevel.Error,
            Message = "Unhandled exception occurred during application processing."
        )
    ]
    public static partial void UnhandledExceptionOccurred(this ILogger logger, Exception ex);

    [LoggerMessage(
            EventId = 251,
            EventName = nameof(DisplayProfileCouldNotBeChanged),
            Level = LogLevel.Error,
            Message = "DisplayProfile could not be changed."
        )
    ]
    public static partial void DisplayProfileCouldNotBeChanged(this ILogger logger);
}