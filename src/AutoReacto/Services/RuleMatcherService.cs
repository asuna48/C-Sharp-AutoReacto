using AutoReacto.Core.Interfaces;
using AutoReacto.Core.Models;
using AutoReacto.Utils.Helpers;
using Microsoft.Extensions.Logging;

namespace AutoReacto.Services;

/// <summary>
/// Service for matching messages against reaction rules
/// </summary>
public sealed class RuleMatcherService : IRuleMatcher
{
    private readonly ILogger<RuleMatcherService> _logger;

    public RuleMatcherService(ILogger<RuleMatcherService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsMatch(string content, ReactionRule rule)
    {
        if (string.IsNullOrEmpty(content) || !rule.Enabled)
            return false;

        foreach (var trigger in rule.TriggerWords)
        {
            var matches = rule.MatchMode switch
            {
                MatchMode.Contains => content.ContainsText(trigger, rule.CaseSensitive),
                MatchMode.Exact => content.EqualsText(trigger, rule.CaseSensitive),
                MatchMode.StartsWith => content.StartsWithText(trigger, rule.CaseSensitive),
                MatchMode.EndsWith => content.EndsWithText(trigger, rule.CaseSensitive),
                MatchMode.Regex => content.MatchesPattern(trigger, rule.CaseSensitive),
                _ => false
            };

            if (matches)
            {
                _logger.LogDebug("Message matched rule '{RuleName}' with trigger '{Trigger}'", rule.Name, trigger);
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public IEnumerable<ReactionRule> GetMatchingRules(
        string content, 
        ulong channelId, 
        ulong userId, 
        IEnumerable<ReactionRule> rules)
    {
        foreach (var rule in rules)
        {
            if (!rule.Enabled)
                continue;

            // Check channel filter
            if (rule.ChannelIds.Count > 0 && !rule.ChannelIds.Contains(channelId))
                continue;

            // Check user filter
            if (rule.UserIds.Count > 0 && !rule.UserIds.Contains(userId))
                continue;

            // Check ignore list
            if (rule.IgnoreUserIds.Contains(userId))
                continue;

            // Check content match
            if (IsMatch(content, rule))
            {
                yield return rule;
            }
        }
    }
}
