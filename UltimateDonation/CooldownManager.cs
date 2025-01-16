using System.Collections.Generic;
using Exiled.API.Features;
using System.Linq;

/// <summary>
/// Developer note:
/// We read the command-limits from Config.GlobalCommandLimits[roleKey].
/// We do NOT do direct deconstruction (C# 7+ might support it if the extension is present,
/// but let's avoid it for compatibility).
/// </summary>
public class CooldownManager
{
    private readonly Dictionary<string, Dictionary<string, int>> _usageLimitsPerRole;
    private readonly Dictionary<string, Dictionary<string, int>> _commandUsages;
    private readonly DonatorPlugin _plugin;

    public CooldownManager(Config config, DonatorPlugin plugin)
    {
        _plugin = plugin;

        // Instead of "var (roleKey, cmdLimits) in config.GlobalCommandLimits", we do a normal foreach:
        _usageLimitsPerRole = config.GlobalCommandLimits;
        _commandUsages = new Dictionary<string, Dictionary<string, int>>();

        // We'll do this:
        foreach (var kvp in _usageLimitsPerRole)
        {
            string roleKey = kvp.Key;
            Dictionary<string, int> cmdLimits = kvp.Value;

            // Optionally log them
            var line = string.Join(", ", cmdLimits.Select(x => $"{x.Key}={x.Value}"));
            _plugin.LogDebug($"[CooldownManager] role '{roleKey}' => {line}");
        }
    }

    public bool CanExecuteCommand(string userId, string roleKey, string command)
    {
        if (!_usageLimitsPerRole.ContainsKey(roleKey))
        {
            _plugin.LogDebug($"[CooldownManager] No usage limits for role '{roleKey}' in GlobalCommandLimits.");
            return false;
        }
        var roleLimits = _usageLimitsPerRole[roleKey];
        if (!roleLimits.ContainsKey(command))
        {
            _plugin.LogDebug($"[CooldownManager] role '{roleKey}' does not define limit for command '{command}'.");
            return false;
        }

        if (!_commandUsages.TryGetValue(userId, out var userDict))
        {
            userDict = new Dictionary<string, int>();
            _commandUsages[userId] = userDict;
        }

        if (!userDict.ContainsKey(command))
            userDict[command] = 0;

        var used = userDict[command];
        var max = roleLimits[command];
        return used < max;
    }

    public void RegisterCommandUsage(string userId, string roleKey, string command)
    {
        if (!_commandUsages.TryGetValue(userId, out var userDict))
        {
            userDict = new Dictionary<string, int>();
            _commandUsages[userId] = userDict;
        }

        if (!userDict.ContainsKey(command))
            userDict[command] = 0;

        userDict[command]++;
        _plugin.LogDebug($"[CooldownManager] {userId} used {command}, now={userDict[command]}");
    }

    public int GetUsageCount(string userId, string command)
    {
        if (_commandUsages.TryGetValue(userId, out var userDict))
        {
            if (userDict.TryGetValue(command, out var used))
                return used;
        }
        return 0;
    }

    public void ResetCooldowns()
    {
        _commandUsages.Clear();
        _plugin.LogDebug("[CooldownManager] All usage counters reset for new round.");
    }
}
