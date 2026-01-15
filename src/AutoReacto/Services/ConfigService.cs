using System.Text.Json;
using System.Text.Json.Serialization;
using AutoReacto.Core.Interfaces;
using AutoReacto.Core.Models;
using Microsoft.Extensions.Logging;

namespace AutoReacto.Services;

/// <summary>
/// Service for managing bot configuration (config.json and emojis.json)
/// </summary>
public sealed class ConfigService : IConfigService, IDisposable
{
    private readonly ILogger<ConfigService> _logger;
    private readonly string _configPath;
    private readonly string _emojisPath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly FileSystemWatcher? _configWatcher;
    private readonly FileSystemWatcher? _emojisWatcher;
    private BotConfig _config;
    private EmojisConfig _emojisConfig;
    private DateTime _lastReload = DateTime.MinValue;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Preserve emoji characters as-is
    };

    public ConfigService(ILogger<ConfigService> logger, string? configPath = null)
    {
        _logger = logger;
        _configPath = configPath ?? FindConfigPath("config.json");
        _emojisPath = FindConfigPath("emojis.json");
        _config = new BotConfig();
        _emojisConfig = new EmojisConfig();
        
        _logger.LogInformation("Using config file: {Path}", _configPath);
        _logger.LogInformation("Using emojis file: {Path}", _emojisPath);
        
        // Setup file watcher for hot-reload (config.json)
        var directory = Path.GetDirectoryName(_configPath);
        
        if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
        {
            // Watch config.json
            _configWatcher = new FileSystemWatcher(directory, "config.json")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };
            _configWatcher.Changed += OnConfigFileChanged;
            
            // Watch emojis.json
            _emojisWatcher = new FileSystemWatcher(directory, "emojis.json")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };
            _emojisWatcher.Changed += OnEmojisFileChanged;
            
            _logger.LogInformation("File watchers enabled for hot-reload");
        }
    }

    /// <summary>
    /// Finds the config file path, preferring source directory in development
    /// </summary>
    private static string FindConfigPath(string fileName)
    {
        var baseDir = AppContext.BaseDirectory;
        
        // Priority 1: Source directory (development mode)
        // From bin/Debug/net8.0 -> go up to src/AutoReacto
        var sourcePath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", fileName));
        if (File.Exists(sourcePath))
        {
            return sourcePath;
        }
        
        // Priority 2: Same directory as executable (production mode)
        var exePath = Path.Combine(baseDir, fileName);
        return exePath;
    }

    private async void OnConfigFileChanged(object sender, FileSystemEventArgs e)
    {
        // Debounce: ignore if reloaded recently (within 1 second)
        if ((DateTime.Now - _lastReload).TotalSeconds < 1)
            return;
            
        _lastReload = DateTime.Now;
        
        _logger.LogInformation("Config file changed, reloading...");
        
        // Wait a bit for file to be fully written
        await Task.Delay(500);
        
        try
        {
            await ReloadConfigAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reload config after file change");
        }
    }

    private async void OnEmojisFileChanged(object sender, FileSystemEventArgs e)
    {
        // Debounce: ignore if reloaded recently (within 1 second)
        if ((DateTime.Now - _lastReload).TotalSeconds < 1)
            return;
            
        _lastReload = DateTime.Now;
        
        _logger.LogInformation("Emojis file changed, reloading...");
        
        // Wait a bit for file to be fully written
        await Task.Delay(500);
        
        try
        {
            await ReloadEmojisAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reload emojis after file change");
        }
    }

    /// <inheritdoc />
    public BotConfig Config => _config;

    /// <summary>
    /// Gets the emojis configuration (reaction rules, custom emojis, frequent emojis)
    /// </summary>
    public EmojisConfig EmojisConfig => _emojisConfig;

    /// <summary>
    /// Gets the reaction rules (convenience property for backwards compatibility)
    /// </summary>
    public List<ReactionRule> ReactionRules => _emojisConfig.ReactionRules;

    /// <inheritdoc />
    public async Task ReloadAsync()
    {
        await ReloadConfigAsync();
        await ReloadEmojisAsync();
    }

    /// <summary>
    /// Reloads only the config.json file
    /// </summary>
    private async Task ReloadConfigAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (!File.Exists(_configPath))
            {
                _logger.LogWarning("Configuration file not found at {Path}. Creating default configuration.", _configPath);
                await CreateDefaultConfigAsync();
                return;
            }

            var json = await File.ReadAllTextAsync(_configPath);
            var config = JsonSerializer.Deserialize<BotConfig>(json, JsonOptions);

            if (config == null)
            {
                _logger.LogError("Failed to deserialize configuration. Using default configuration.");
                _config = new BotConfig();
                return;
            }

            // Validate token
            if (string.IsNullOrWhiteSpace(config.Token))
            {
                _logger.LogWarning("Bot token is not configured. Please update config.json with your Discord bot token.");
            }

            _config = config;
            _logger.LogInformation("Configuration loaded successfully.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse configuration file. Please check the JSON syntax.");
            throw;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Reloads only the emojis.json file
    /// </summary>
    private async Task ReloadEmojisAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (!File.Exists(_emojisPath))
            {
                _logger.LogWarning("Emojis file not found at {Path}. Creating default emojis configuration.", _emojisPath);
                await CreateDefaultEmojisAsync();
                return;
            }

            var json = await File.ReadAllTextAsync(_emojisPath);
            var emojisConfig = JsonSerializer.Deserialize<EmojisConfig>(json, JsonOptions);

            if (emojisConfig == null)
            {
                _logger.LogError("Failed to deserialize emojis configuration. Using default.");
                _emojisConfig = new EmojisConfig();
                return;
            }

            _emojisConfig = emojisConfig;
            _logger.LogInformation("Emojis configuration loaded successfully. {RuleCount} reaction rules found.", _emojisConfig.ReactionRules.Count);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse emojis file. Please check the JSON syntax.");
            throw;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync()
    {
        await SaveConfigAsync();
        await SaveEmojisAsync();
    }

    /// <summary>
    /// Saves only the config.json file
    /// </summary>
    public async Task SaveConfigAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(_config, JsonOptions);
            await File.WriteAllTextAsync(_configPath, json);
            _logger.LogInformation("Configuration saved successfully.");
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Saves only the emojis.json file
    /// </summary>
    public async Task SaveEmojisAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(_emojisConfig, JsonOptions);
            await File.WriteAllTextAsync(_emojisPath, json);
            _logger.LogInformation("Emojis configuration saved successfully.");
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Creates a default configuration file
    /// </summary>
    private async Task CreateDefaultConfigAsync()
    {
        _config = new BotConfig
        {
            Token = "YOUR_BOT_TOKEN_HERE",
            Prefix = "!",
            Settings = new GlobalSettings
            {
                IgnoreBots = true,
                IgnoreSelf = true,
                ReactionDelayMs = 250,
                MaxReactionsPerMessage = 20,
                LogLevel = "Information"
            }
        };

        await SaveConfigAsync();
        _logger.LogInformation("Default configuration file created at {Path}. Please update with your bot token.", _configPath);
    }

    /// <summary>
    /// Creates a default emojis configuration file
    /// </summary>
    private async Task CreateDefaultEmojisAsync()
    {
        _emojisConfig = new EmojisConfig
        {
            ReactionRules = new List<ReactionRule>
            {
                new()
                {
                    Id = "example-rule-1",
                    Name = "Hello Reaction",
                    Enabled = true,
                    TriggerWords = new List<string> { "hello", "hi", "merhaba" },
                    Emojis = new List<string> { "👋", "😊" },
                    MatchMode = MatchMode.Contains,
                    CaseSensitive = false
                },
                new()
                {
                    Id = "example-rule-2",
                    Name = "Heart Reaction",
                    Enabled = true,
                    TriggerWords = new List<string> { "love", "aşk", "❤️" },
                    Emojis = new List<string> { "❤️", "💕" },
                    MatchMode = MatchMode.Contains,
                    CaseSensitive = false
                }
            },
            CustomEmojis = new List<string>(),
            FrequentEmojis = new List<string> { "👋", "😊", "❤️", "💕", "😂", "🎉" }
        };

        await SaveEmojisAsync();
        _logger.LogInformation("Default emojis configuration file created at {Path}.", _emojisPath);
    }

    public void Dispose()
    {
        _configWatcher?.Dispose();
        _emojisWatcher?.Dispose();
        _lock.Dispose();
    }
}
