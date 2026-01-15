using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoReacto.Dashboard.Models;

/// <summary>
/// Manages emoji storage in a separate JSON file
/// </summary>
public class EmojiStorage
{
    private static string? _emojiFilePath;
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Preserve emoji characters
    };

    /// <summary>
    /// Dictionary mapping rule ID to list of emojis
    /// </summary>
    public Dictionary<string, List<string>> RuleEmojis { get; set; } = new();

    /// <summary>
    /// Custom emojis added by user (Discord format)
    /// </summary>
    public List<string> CustomEmojis { get; set; } = new();

    /// <summary>
    /// Frequently used emojis (auto-tracked)
    /// </summary>
    public List<string> FrequentEmojis { get; set; } = new();

    /// <summary>
    /// Set the emoji file path based on config path
    /// </summary>
    public static void SetBasePath(string configPath)
    {
        var dir = Path.GetDirectoryName(configPath);
        if (!string.IsNullOrEmpty(dir))
        {
            _emojiFilePath = Path.Combine(dir, "emojis.json");
        }
    }

    private static string GetEmojiFilePath()
    {
        if (!string.IsNullOrEmpty(_emojiFilePath))
            return _emojiFilePath;
            
        // Fallback: Same directory as app
        return Path.Combine(AppContext.BaseDirectory, "emojis.json");
    }

    /// <summary>
    /// Load emojis from JSON file
    /// </summary>
    public static EmojiStorage Load()
    {
        try
        {
            var path = GetEmojiFilePath();
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var storage = JsonSerializer.Deserialize<EmojiStorage>(json, JsonOptions);
                return storage ?? new EmojiStorage();
            }
        }
        catch
        {
            // Return empty storage on error
        }
        
        return new EmojiStorage();
    }

    /// <summary>
    /// Save emojis to JSON file
    /// </summary>
    public void Save()
    {
        try
        {
            var path = GetEmojiFilePath();
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            var json = JsonSerializer.Serialize(this, JsonOptions);
            File.WriteAllText(path, json);
            
            System.Diagnostics.Debug.WriteLine($"Emojis saved to: {path}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save emojis: {ex.Message}");
        }
    }

    /// <summary>
    /// Get emojis for a specific rule
    /// </summary>
    public List<string> GetEmojisForRule(string ruleId)
    {
        if (RuleEmojis.TryGetValue(ruleId, out var emojis))
        {
            return emojis;
        }
        return new List<string>();
    }

    /// <summary>
    /// Set emojis for a specific rule
    /// </summary>
    public void SetEmojisForRule(string ruleId, List<string> emojis)
    {
        RuleEmojis[ruleId] = emojis;
    }

    /// <summary>
    /// Track emoji usage for frequently used list
    /// </summary>
    public void TrackEmojiUsage(string emoji)
    {
        // Remove if exists and add to front
        FrequentEmojis.Remove(emoji);
        FrequentEmojis.Insert(0, emoji);
        
        // Keep only last 20
        if (FrequentEmojis.Count > 20)
        {
            FrequentEmojis = FrequentEmojis.Take(20).ToList();
        }
    }

    /// <summary>
    /// Add a custom emoji
    /// </summary>
    public void AddCustomEmoji(string emoji)
    {
        if (!CustomEmojis.Contains(emoji))
        {
            CustomEmojis.Add(emoji);
        }
    }
}
