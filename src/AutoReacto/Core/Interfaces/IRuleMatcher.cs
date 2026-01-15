using AutoReacto.Core.Models;

namespace AutoReacto.Core.Interfaces;

/// <summary>
/// Interface for matching messages against rules
/// </summary>
public interface IRuleMatcher
{
    /// <summary>
    /// Checks if a message matches a reaction rule
    /// </summary>
    /// <param name="content">Message content</param>
    /// <param name="rule">Reaction rule to check</param>
    /// <returns>True if message matches the rule</returns>
    bool IsMatch(string content, ReactionRule rule);

    /// <summary>
    /// Gets all matching rules for a message
    /// </summary>
    /// <param name="content">Message content</param>
    /// <param name="channelId">Channel ID</param>
    /// <param name="userId">User ID</param>
    /// <param name="rules">Available rules</param>
    /// <returns>Matching rules</returns>
    IEnumerable<ReactionRule> GetMatchingRules(
        string content, 
        ulong channelId, 
        ulong userId, 
        IEnumerable<ReactionRule> rules);
}
