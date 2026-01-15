using Discord;
using Discord.WebSocket;
using AutoReacto.Core.Interfaces;
using AutoReacto.Utils.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoReacto.Services;

/// <summary>
/// Main Discord bot service that handles connection and events
/// </summary>
public sealed class BotService : IHostedService, IDisposable
{
    private readonly ILogger<BotService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly IConfigService _configService;
    private readonly IReactionHandler _reactionHandler;
    private readonly DiscordLogAdapter _logAdapter;

    public BotService(
        ILogger<BotService> logger,
        DiscordSocketClient client,
        IConfigService configService,
        IReactionHandler reactionHandler,
        DiscordLogAdapter logAdapter)
    {
        _logger = logger;
        _client = client;
        _configService = configService;
        _reactionHandler = reactionHandler;
        _logAdapter = logAdapter;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting AutoReacto bot...");

        // Load configuration
        await _configService.ReloadAsync();

        var config = _configService.Config;
        
        if (string.IsNullOrWhiteSpace(config.Token) || config.Token == "YOUR_BOT_TOKEN_HERE")
        {
            _logger.LogError("Bot token is not configured. Please update config.json with your Discord bot token.");
            throw new InvalidOperationException("Bot token is not configured.");
        }

        // Register event handlers
        _client.Log += _logAdapter.LogAsync;
        _client.Ready += OnReadyAsync;
        _client.MessageReceived += OnMessageReceivedAsync;
        _client.Disconnected += OnDisconnectedAsync;

        // Login and start
        await _client.LoginAsync(TokenType.Bot, config.Token);
        await _client.StartAsync();

        _logger.LogInformation("Bot started successfully.");
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping AutoReacto bot...");

        // Unregister event handlers
        _client.Log -= _logAdapter.LogAsync;
        _client.Ready -= OnReadyAsync;
        _client.MessageReceived -= OnMessageReceivedAsync;
        _client.Disconnected -= OnDisconnectedAsync;

        await _client.StopAsync();
        await _client.LogoutAsync();

        _logger.LogInformation("Bot stopped.");
    }

    /// <summary>
    /// Called when the bot is ready and connected
    /// </summary>
    private Task OnReadyAsync()
    {
        _logger.LogInformation("Bot connected as {Username}#{Discriminator}", 
            _client.CurrentUser.Username, 
            _client.CurrentUser.Discriminator);
        
        _logger.LogInformation("Connected to {GuildCount} guild(s)", _client.Guilds.Count);

        foreach (var guild in _client.Guilds)
        {
            _logger.LogDebug("Connected to guild: {GuildName} ({GuildId})", guild.Name, guild.Id);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when a message is received
    /// </summary>
    private async Task OnMessageReceivedAsync(SocketMessage message)
    {
        // Ignore own messages
        if (message.Author.Id == _client.CurrentUser.Id && _configService.Config.Settings.IgnoreSelf)
            return;

        try
        {
            await _reactionHandler.HandleMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message {MessageId}", message.Id);
        }
    }

    /// <summary>
    /// Called when the bot disconnects
    /// </summary>
    private Task OnDisconnectedAsync(Exception exception)
    {
        if (exception != null)
        {
            _logger.LogWarning(exception, "Bot disconnected unexpectedly.");
        }
        else
        {
            _logger.LogInformation("Bot disconnected.");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _client.Dispose();
    }
}
