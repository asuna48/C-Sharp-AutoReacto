using System.Text.RegularExpressions;

namespace AutoReacto.Utils.Helpers;

/// <summary>
/// Helper utilities for string operations
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Checks if a string contains another string with optional case sensitivity
    /// </summary>
    public static bool ContainsText(this string source, string value, bool caseSensitive = false)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
            return false;

        var comparison = caseSensitive 
            ? StringComparison.Ordinal 
            : StringComparison.OrdinalIgnoreCase;

        return source.Contains(value, comparison);
    }

    /// <summary>
    /// Checks if a string starts with another string with optional case sensitivity
    /// </summary>
    public static bool StartsWithText(this string source, string value, bool caseSensitive = false)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
            return false;

        var comparison = caseSensitive 
            ? StringComparison.Ordinal 
            : StringComparison.OrdinalIgnoreCase;

        return source.StartsWith(value, comparison);
    }

    /// <summary>
    /// Checks if a string ends with another string with optional case sensitivity
    /// </summary>
    public static bool EndsWithText(this string source, string value, bool caseSensitive = false)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
            return false;

        var comparison = caseSensitive 
            ? StringComparison.Ordinal 
            : StringComparison.OrdinalIgnoreCase;

        return source.EndsWith(value, comparison);
    }

    /// <summary>
    /// Checks if two strings are equal with optional case sensitivity
    /// </summary>
    public static bool EqualsText(this string source, string value, bool caseSensitive = false)
    {
        if (source == null && value == null) return true;
        if (source == null || value == null) return false;

        var comparison = caseSensitive 
            ? StringComparison.Ordinal 
            : StringComparison.OrdinalIgnoreCase;

        return source.Equals(value, comparison);
    }

    /// <summary>
    /// Checks if a string matches a regex pattern
    /// </summary>
    public static bool MatchesPattern(this string source, string pattern, bool caseSensitive = false)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(pattern))
            return false;

        try
        {
            var options = caseSensitive 
                ? RegexOptions.None 
                : RegexOptions.IgnoreCase;

            return Regex.IsMatch(source, pattern, options, TimeSpan.FromMilliseconds(100));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
