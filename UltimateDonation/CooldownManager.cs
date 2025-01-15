using System.Collections.Generic;
using Exiled.API.Features;

public class CooldownManager
{
    // Храним для каждой роли — для каждой команды — максимально допустимое число в раунд
    private readonly Dictionary<string, Dictionary<string, int>> _usageLimits;
    // А тут считаем фактически использованное кол-во команд конкретным игроком
    // userId -> (command -> current usage)
    private readonly Dictionary<string, Dictionary<string, int>> _commandUsages;

    public CooldownManager(Config config)
    {
        _usageLimits = new Dictionary<string, Dictionary<string, int>>();
        _commandUsages = new Dictionary<string, Dictionary<string, int>>();

        // Считываем лимиты из DonatorRoles
        foreach (var kvp in config.DonatorRoles)
        {
            var roleName = kvp.Key;
            var donatorRole = kvp.Value;

            _usageLimits[roleName] = new Dictionary<string, int>();
            // Для каждой команды, прописанной в CommandLimits, запоминаем лимит
            foreach (var commandLimit in donatorRole.CommandLimits)
            {
                _usageLimits[roleName][commandLimit.Key] = commandLimit.Value;
            }
        }
    }

    public bool CanExecuteCommand(string userId, string role, string command)
    {
        // Если в лимитах такой роли вообще нет нужной команды — значит, нельзя
        if (!_usageLimits.ContainsKey(role) || !_usageLimits[role].ContainsKey(command))
            return false;

        if (!_commandUsages.ContainsKey(userId))
            _commandUsages[userId] = new Dictionary<string, int>();

        if (!_commandUsages[userId].ContainsKey(command))
            _commandUsages[userId][command] = 0;

        // Сравниваем текущее число использований с максимально допустимым
        return _commandUsages[userId][command] < _usageLimits[role][command];
    }

    public void RegisterCommandUsage(string userId, string role, string command)
    {
        if (!_commandUsages.ContainsKey(userId))
            _commandUsages[userId] = new Dictionary<string, int>();

        if (!_commandUsages[userId].ContainsKey(command))
            _commandUsages[userId][command] = 0;

        _commandUsages[userId][command]++;
    }

    public void ResetCooldowns()
    {
        // Сбрасываем все счётчики использований в начале (или конце) каждого раунда
        _commandUsages.Clear();
    }
}
