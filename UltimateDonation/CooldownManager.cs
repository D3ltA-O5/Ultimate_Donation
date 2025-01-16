using System.Collections.Generic;
using Exiled.API.Features;

public class CooldownManager
{
    private readonly Dictionary<string, Dictionary<string, int>> _usageLimits;
    private readonly Dictionary<string, Dictionary<string, int>> _commandUsages;
    private readonly DonatorPlugin _plugin;

    public CooldownManager(Config config, DonatorPlugin plugin)
    {
        _plugin = plugin;
        _usageLimits = new Dictionary<string, Dictionary<string, int>>();
        _commandUsages = new Dictionary<string, Dictionary<string, int>>();

        // Считываем лимиты команд из config.UltimateDonation.donator_roles
        foreach (var kvp in config.UltimateDonation.donator_roles)
        {
            var roleName = kvp.Key;
            var roleData = kvp.Value;

            _usageLimits[roleName] = new Dictionary<string, int>();
            // roleData.command_limits
            foreach (var cmdLimit in roleData.command_limits)
            {
                _usageLimits[roleName][cmdLimit.Key] = cmdLimit.Value;
            }

            _plugin.LogDebug($"[CooldownManager] Role '{roleName}' command limits: {string.Join(", ", roleData.command_limits)}");
        }
    }

    // Проверка, можно ли ещё использовать команду
    public bool CanExecuteCommand(string userId, string role, string command)
    {
        if (!_usageLimits.ContainsKey(role))
        {
            _plugin.LogDebug($"[CooldownManager] Role '{role}' not found in usage limits.");
            return false;
        }
        if (!_usageLimits[role].ContainsKey(command))
        {
            _plugin.LogDebug($"[CooldownManager] Command '{command}' not listed for role '{role}'.");
            return false;
        }

        if (!_commandUsages.ContainsKey(userId))
            _commandUsages[userId] = new Dictionary<string, int>();

        if (!_commandUsages[userId].ContainsKey(command))
            _commandUsages[userId][command] = 0;

        int current = _commandUsages[userId][command];
        int max = _usageLimits[role][command];
        _plugin.LogDebug($"[CooldownManager] Checking usage {userId}/{role}/{command}: {current} of {max}");

        return current < max;
    }

    // Регистрируем использование команды
    public void RegisterCommandUsage(string userId, string role, string command)
    {
        if (!_commandUsages.ContainsKey(userId))
            _commandUsages[userId] = new Dictionary<string, int>();

        if (!_commandUsages[userId].ContainsKey(command))
            _commandUsages[userId][command] = 0;

        _commandUsages[userId][command]++;
        _plugin.LogDebug($"[CooldownManager] {userId} used '{command}' for role {role}, total uses: {_commandUsages[userId][command]}");
    }

    // Сбросить счётчики в конце/начале раунда
    public void ResetCooldowns()
    {
        _commandUsages.Clear();
        _plugin.LogDebug("[CooldownManager] Reset all command usage counters.");
    }
}
