using Discord;
using Microsoft.Extensions.Logging;

namespace AutoReacto.Utils.Logging;

/// <summary>
/// Adapter to convert Discord.Net logs to Microsoft.Extensions.Logging
/// </summary>
public class DiscordLogAdapter
{
    private readonly ILogger<DiscordLogAdapter> _logger;

    public DiscordLogAdapter(ILogger<DiscordLogAdapter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles Discord.Net log messages
    /// </summary>
    public Task LogAsync(LogMessage message)
    {
        var logLevel = ConvertLogSeverity(message.Severity);
        
        if (message.Exception != null)
        {
            _logger.Log(logLevel, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        }
        else
        {
            _logger.Log(logLevel, "[{Source}] {Message}", message.Source, message.Message);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Converts Discord LogSeverity to Microsoft LogLevel
    /// </summary>
    private static LogLevel ConvertLogSeverity(LogSeverity severity)
    {
        return severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.Information
        };
    }
}
