namespace AutoReacto.Core.Models;

/// <summary>
/// Represents the bot configuration loaded from config.json
/// </summary>
public sealed class BotConfig
{
    /// <summary>
    /// Discord bot token from Discord Developer Portal
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Bot command prefix (e.g., "!")
    /// </summary>
    public string Prefix { get; set; } = "!";

    /// <summary>
    /// Global settings for the bot
    /// </summary>
    public GlobalSettings Settings { get; set; } = new();
}

/// <summary>
/// Represents the emojis/reaction rules configuration loaded from emojis.json
/// </summary>
public sealed class EmojisConfig
{
    /// <summary>
    /// List of reaction rules
    /// </summary>
    public List<ReactionRule> ReactionRules { get; set; } = new();

    /// <summary>
    /// Custom Discord emojis (format: &lt;:name:id&gt;)
    /// </summary>
    public List<string> CustomEmojis { get; set; } = new();

    /// <summary>
    /// Frequently used emojis for quick access
    /// </summary>
    public List<string> FrequentEmojis { get; set; } = new();
}

/// <summary>
/// Represents a single reaction rule
/// </summary>
public sealed class ReactionRule
{
    /// <summary>
    /// Unique identifier for this rule
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Rule name for easy identification
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Whether this rule is currently active
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Trigger words/phrases that activate this rule
    /// </summary>
    public List<string> TriggerWords { get; set; } = new();

    /// <summary>
    /// Emojis to react with (can be Unicode or custom emoji format)
    /// </summary>
    public List<string> Emojis { get; set; } = new();

    /// <summary>
    /// Channel IDs where this rule applies (empty = all channels)
    /// </summary>
    public List<ulong> ChannelIds { get; set; } = new();

    /// <summary>
    /// User IDs to target (empty = all users)
    /// </summary>
    public List<ulong> UserIds { get; set; } = new();

    /// <summary>
    /// User IDs to ignore
    /// </summary>
    public List<ulong> IgnoreUserIds { get; set; } = new();

    /// <summary>
    /// Whether to match case-sensitively
    /// </summary>
    public bool CaseSensitive { get; set; } = false;

    /// <summary>
    /// Match mode: Contains, Exact, StartsWith, EndsWith, Regex
    /// </summary>
    public MatchMode MatchMode { get; set; } = MatchMode.Contains;
}

/// <summary>
/// Match mode for trigger words
/// </summary>
public enum MatchMode
{
    /// <summary>Message contains the trigger word</summary>
    Contains,
    /// <summary>Message exactly matches the trigger word</summary>
    Exact,
    /// <summary>Message starts with the trigger word</summary>
    StartsWith,
    /// <summary>Message ends with the trigger word</summary>
    EndsWith,
    /// <summary>Trigger word is treated as a regex pattern</summary>
    Regex
}

/// <summary>
/// Global bot settings
/// </summary>
public sealed class GlobalSettings
{
    /// <summary>
    /// Whether to ignore bot messages
    /// </summary>
    public bool IgnoreBots { get; set; } = true;

    /// <summary>
    /// Whether to ignore the bot's own messages
    /// </summary>
    public bool IgnoreSelf { get; set; } = true;

    /// <summary>
    /// Delay between adding multiple reactions (in milliseconds)
    /// </summary>
    public int ReactionDelayMs { get; set; } = 250;

    /// <summary>
    /// Maximum reactions per message
    /// </summary>
    public int MaxReactionsPerMessage { get; set; } = 20;

    /// <summary>
    /// Log level: Debug, Information, Warning, Error
    /// </summary>
    public string LogLevel { get; set; } = "Information";
}
