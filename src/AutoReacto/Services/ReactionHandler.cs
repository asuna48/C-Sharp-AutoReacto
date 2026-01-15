using Discord;
using Discord.WebSocket;
using AutoReacto.Core.Interfaces;
using AutoReacto.Utils.Helpers;
using Microsoft.Extensions.Logging;

namespace AutoReacto.Services;

/// <summary>
/// Handles adding reactions to messages based on configured rules
/// </summary>
public sealed class ReactionHandler : IReactionHandler
{
    private readonly ILogger<ReactionHandler> _logger;
    private readonly IConfigService _configService;
    private readonly IRuleMatcher _ruleMatcher;

    public ReactionHandler(
        ILogger<ReactionHandler> logger,
        IConfigService configService,
        IRuleMatcher ruleMatcher)
    {
        _logger = logger;
        _configService = configService;
        _ruleMatcher = ruleMatcher;
    }

    /// <inheritdoc />
    public async Task<int> HandleMessageAsync(SocketMessage message)
    {
        // Ignore system messages
        if (message is not SocketUserMessage userMessage)
            return 0;

        var config = _configService.Config;

        // Apply global filters
        if (config.Settings.IgnoreBots && message.Author.IsBot)
            return 0;

        if (config.Settings.IgnoreSelf && message.Author.Id == message.Author.Id)
        {
            // This check should compare with bot's own ID, handled in BotService
        }

        // Get channel ID
        var channelId = message.Channel.Id;
        var userId = message.Author.Id;
        var content = message.Content;

        // Find matching rules (reaction rules are now in emojis.json)
        var matchingRules = _ruleMatcher.GetMatchingRules(
            content, 
            channelId, 
            userId, 
            _configService.ReactionRules).ToList();

        if (matchingRules.Count == 0)
            return 0;

        _logger.LogDebug("Found {Count} matching rules for message from {Author}", 
            matchingRules.Count, message.Author.Username);

        // Collect all emojis to add
        var emojisToAdd = new List<IEmote>();
        foreach (var rule in matchingRules)
        {
            var parsedEmojis = EmojiHelper.ParseEmojis(rule.Emojis);
            emojisToAdd.AddRange(parsedEmojis);
        }

        // Apply max reactions limit
        if (emojisToAdd.Count > config.Settings.MaxReactionsPerMessage)
        {
            emojisToAdd = emojisToAdd.Take(config.Settings.MaxReactionsPerMessage).ToList();
        }

        // Remove duplicates
        emojisToAdd = emojisToAdd.DistinctBy(e => e.ToString()).ToList();

        // Add reactions with delay
        var reactionsAdded = 0;
        foreach (var emoji in emojisToAdd)
        {
            try
            {
                await userMessage.AddReactionAsync(emoji);
                reactionsAdded++;
                
                _logger.LogDebug("Added reaction {Emoji} to message {MessageId}", 
                    EmojiHelper.FormatEmote(emoji), message.Id);

                // Apply delay between reactions to avoid rate limiting
                if (config.Settings.ReactionDelayMs > 0 && reactionsAdded < emojisToAdd.Count)
                {
                    await Task.Delay(config.Settings.ReactionDelayMs);
                }
            }
            catch (Discord.Net.HttpException ex) when (ex.HttpCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogWarning("Missing permission to add reactions in channel {Channel}", message.Channel.Name);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add reaction {Emoji} to message {MessageId}", 
                    EmojiHelper.FormatEmote(emoji), message.Id);
            }
        }

        if (reactionsAdded > 0)
        {
            _logger.LogInformation("Added {Count} reactions to message from {Author} in #{Channel}", 
                reactionsAdded, message.Author.Username, message.Channel.Name);
        }

        return reactionsAdded;
    }
}
