using Discord;

namespace AutoReacto.Utils.Helpers;

/// <summary>
/// Helper utilities for emoji parsing and validation
/// </summary>
public static class EmojiHelper
{
    /// <summary>
    /// Parses an emoji string to an IEmote (supports Unicode and custom emojis)
    /// </summary>
    /// <param name="emojiString">Emoji string (Unicode or &lt;:name:id&gt; format)</param>
    /// <returns>Parsed IEmote or null if invalid</returns>
    public static IEmote? ParseEmoji(string emojiString)
    {
        if (string.IsNullOrWhiteSpace(emojiString))
            return null;

        emojiString = emojiString.Trim();

        // Try parsing as custom emoji first (format: <:name:id> or <a:name:id>)
        if (Emote.TryParse(emojiString, out var emote))
        {
            return emote;
        }

        // Try as Unicode emoji
        if (IsValidUnicodeEmoji(emojiString))
        {
            return new Emoji(emojiString);
        }

        return null;
    }

    /// <summary>
    /// Parses multiple emoji strings
    /// </summary>
    /// <param name="emojiStrings">Collection of emoji strings</param>
    /// <returns>Valid emotes</returns>
    public static IEnumerable<IEmote> ParseEmojis(IEnumerable<string> emojiStrings)
    {
        foreach (var emojiString in emojiStrings)
        {
            var emote = ParseEmoji(emojiString);
            if (emote != null)
            {
                yield return emote;
            }
        }
    }

    /// <summary>
    /// Checks if a string appears to be a valid Unicode emoji
    /// </summary>
    private static bool IsValidUnicodeEmoji(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        // If it contains only ASCII letters/numbers/spaces, it's not an emoji
        if (input.All(c => c < 128 && (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))))
            return false;

        // Basic validation - check if it contains emoji-like code points
        var codePoints = GetCodePoints(input).ToList();
        
        if (codePoints.Count == 0)
            return false;

        // At least one code point should be in emoji ranges
        return codePoints.Any(IsEmojiCodePoint);
    }

    /// <summary>
    /// Gets Unicode code points from a string
    /// </summary>
    private static IEnumerable<int> GetCodePoints(string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            int codePoint = char.ConvertToUtf32(input, i);
            if (char.IsHighSurrogate(input[i]))
            {
                i++;
            }
            yield return codePoint;
        }
    }

    /// <summary>
    /// Checks if a code point is in common emoji ranges
    /// </summary>
    private static bool IsEmojiCodePoint(int codePoint)
    {
        // Comprehensive emoji Unicode ranges
        return codePoint is
            // Emoticons
            >= 0x1F600 and <= 0x1F64F or
            // Miscellaneous Symbols and Pictographs  
            >= 0x1F300 and <= 0x1F5FF or
            // Transport and Map Symbols
            >= 0x1F680 and <= 0x1F6FF or
            // Supplemental Symbols and Pictographs
            >= 0x1F900 and <= 0x1F9FF or
            // Symbols and Pictographs Extended-A
            >= 0x1FA00 and <= 0x1FA6F or
            // Symbols and Pictographs Extended-B
            >= 0x1FA70 and <= 0x1FAFF or
            // Miscellaneous Symbols
            >= 0x2600 and <= 0x26FF or
            // Dingbats
            >= 0x2700 and <= 0x27BF or
            // Enclosed Alphanumeric Supplement
            >= 0x1F100 and <= 0x1F1FF or
            // Regional Indicator Symbols (Flags)
            >= 0x1F1E0 and <= 0x1F1FF or
            // Variation Selectors
            >= 0xFE00 and <= 0xFE0F or
            // Zero Width Joiner
            0x200D or
            // Combining Enclosing Keycap
            0x20E3 or
            // Tags
            >= 0xE0020 and <= 0xE007F or
            // Arrows
            >= 0x2190 and <= 0x21FF or
            // Mathematical Operators (some used as emoji)
            >= 0x2200 and <= 0x22FF or
            // Box Drawing (some platforms render as emoji)
            >= 0x25A0 and <= 0x25FF or
            // Geometric Shapes
            >= 0x25A0 and <= 0x25FF or
            // CJK Symbols (some render as emoji)
            >= 0x3000 and <= 0x303F or
            // Copyright, Trademark, etc.
            0x00A9 or 0x00AE or
            // Other common emoji code points
            0x203C or 0x2049 or  // ‼️ ⁉️
            0x2122 or            // ™
            0x2139 or            // ℹ️
            >= 0x2194 and <= 0x2199 or  // Arrows
            >= 0x21A9 and <= 0x21AA or  // Arrows
            >= 0x231A and <= 0x231B or  // Watch, Hourglass
            0x2328 or            // Keyboard
            0x23CF or            // Eject
            >= 0x23E9 and <= 0x23F3 or  // Media controls
            >= 0x23F8 and <= 0x23FA or  // Media controls
            0x24C2 or            // Ⓜ️
            >= 0x25AA and <= 0x25AB or  // Squares
            0x25B6 or 0x25C0 or  // Play buttons
            >= 0x25FB and <= 0x25FE or  // Squares
            >= 0x2614 and <= 0x2615 or  // Umbrella, Coffee
            >= 0x2648 and <= 0x2653 or  // Zodiac
            0x267F or            // Wheelchair
            0x2693 or            // Anchor
            0x26A1 or            // High Voltage
            >= 0x26AA and <= 0x26AB or  // Circles
            >= 0x26BD and <= 0x26BE or  // Soccer, Baseball
            >= 0x26C4 and <= 0x26C5 or  // Snowman, Sun
            0x26CE or            // Ophiuchus
            0x26D4 or            // No Entry
            0x26EA or            // Church
            >= 0x26F2 and <= 0x26F3 or  // Fountain, Golf
            0x26F5 or            // Sailboat
            0x26FA or            // Tent
            0x26FD or            // Fuel Pump
            >= 0x2702 and <= 0x2708;    // Scissors to Airplane
    }

    /// <summary>
    /// Formats an emote for display
    /// </summary>
    public static string FormatEmote(IEmote emote)
    {
        return emote switch
        {
            Emote customEmote => $"<:{customEmote.Name}:{customEmote.Id}>",
            Emoji emoji => emoji.Name,
            _ => emote.ToString() ?? string.Empty
        };
    }
}
