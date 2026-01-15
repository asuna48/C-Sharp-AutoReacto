namespace AutoReacto.Core.Interfaces;

/// <summary>
/// Interface for configuration management
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// Gets the current bot configuration (token, prefix, settings)
    /// </summary>
    Models.BotConfig Config { get; }

    /// <summary>
    /// Gets the emojis configuration (reaction rules, custom emojis, frequent emojis)
    /// </summary>
    Models.EmojisConfig EmojisConfig { get; }

    /// <summary>
    /// Gets the reaction rules (convenience property)
    /// </summary>
    List<Models.ReactionRule> ReactionRules { get; }

    /// <summary>
    /// Reloads all configuration from files (config.json and emojis.json)
    /// </summary>
    Task ReloadAsync();

    /// <summary>
    /// Saves all configuration to files
    /// </summary>
    Task SaveAsync();

    /// <summary>
    /// Saves only the config.json file
    /// </summary>
    Task SaveConfigAsync();

    /// <summary>
    /// Saves only the emojis.json file
    /// </summary>
    Task SaveEmojisAsync();
}
