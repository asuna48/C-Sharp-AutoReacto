using Discord;
using Discord.WebSocket;
using AutoReacto.Core.Interfaces;
using AutoReacto.Services;
using AutoReacto.Utils.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoReacto.Core.Extensions;

/// <summary>
/// Extension methods for service registration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all AutoReacto services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddAutoReactoServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Discord client configuration
        var discordConfig = new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 100,
            GatewayIntents = GatewayIntents.Guilds | 
                            GatewayIntents.GuildMessages | 
                            GatewayIntents.MessageContent |
                            GatewayIntents.GuildMessageReactions,
            AlwaysDownloadUsers = false,
            UseInteractionSnowflakeDate = false
        };

        // Register Discord client as singleton
        services.AddSingleton(discordConfig);
        services.AddSingleton<DiscordSocketClient>();

        // Register logging adapter
        services.AddSingleton<DiscordLogAdapter>();

        // Register configuration service
        services.AddSingleton<IConfigService, ConfigService>();

        // Register core services
        services.AddSingleton<IRuleMatcher, RuleMatcherService>();
        services.AddSingleton<IReactionHandler, ReactionHandler>();

        // Register hosted service (main bot service)
        services.AddHostedService<BotService>();

        return services;
    }
}
