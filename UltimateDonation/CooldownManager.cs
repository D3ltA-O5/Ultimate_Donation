using System;
using System.Collections.Generic;

public class CooldownManager
{
    // Хранение времени отката команд для каждого пользователя
    private readonly Dictionary<string, Dictionary<string, DateTime>> _commandCooldowns =
        new Dictionary<string, Dictionary<string, DateTime>>();

    /// <summary>
    /// Проверяет, может ли пользователь выполнить команду.
    /// </summary>
    /// <param name="userId">ID пользователя.</param>
    /// <param name="commandName">Имя команды.</param>
    /// <returns>True, если команда доступна, иначе False.</returns>
    public bool CanExecuteCommand(string userId, string commandName)
    {
        // Если у пользователя нет записанных команд или указанной команды нет, выполнение разрешено
        if (!_commandCooldowns.ContainsKey(userId) || !_commandCooldowns[userId].ContainsKey(commandName))
            return true;

        // Проверяем, истекло ли время отката
        return _commandCooldowns[userId][commandName] <= DateTime.Now;
    }

    /// <summary>
    /// Регистрирует использование команды пользователем и устанавливает время отката.
    /// </summary>
    /// <param name="userId">ID пользователя.</param>
    /// <param name="commandName">Имя команды.</param>
    /// <param name="cooldownSeconds">Длительность отката в секундах.</param>
    public void RegisterCommandUsage(string userId, string commandName, int cooldownSeconds = 30)
    {
        // Если у пользователя еще нет записей команд, создаем новый словарь
        if (!_commandCooldowns.ContainsKey(userId))
            _commandCooldowns[userId] = new Dictionary<string, DateTime>();

        // Устанавливаем время отката для указанной команды
        _commandCooldowns[userId][commandName] = DateTime.Now.AddSeconds(cooldownSeconds);
    }
}
