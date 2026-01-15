using Serilog;
using Serilog.Events;

namespace AutoReacto.Utils.Logging;

/// <summary>
/// Factory for creating and configuring Serilog loggers
/// </summary>
public static class LoggerFactory
{
    private const string LogDirectory = "logs";
    private const string LogFileTemplate = "autoreacto-.log";
    private const string OutputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Creates a configured Serilog logger instance
    /// </summary>
    /// <param name="logLevel">Minimum log level</param>
    /// <returns>Configured logger</returns>
    public static ILogger CreateLogger(string logLevel = "Information")
    {
        var minLevel = ParseLogLevel(logLevel);

        return new LoggerConfiguration()
            .MinimumLevel.Is(minLevel)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Discord", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: OutputTemplate,
                restrictedToMinimumLevel: minLevel)
            .WriteTo.File(
                path: Path.Combine(LogDirectory, LogFileTemplate),
                outputTemplate: OutputTemplate,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                restrictedToMinimumLevel: minLevel,
                shared: true)
            .CreateLogger();
    }

    /// <summary>
    /// Parses a log level string to LogEventLevel
    /// </summary>
    private static LogEventLevel ParseLogLevel(string logLevel)
    {
        return logLevel.ToLowerInvariant() switch
        {
            "debug" or "verbose" => LogEventLevel.Debug,
            "information" or "info" => LogEventLevel.Information,
            "warning" or "warn" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}
